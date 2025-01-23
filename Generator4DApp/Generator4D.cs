using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace Generator4D
{
    public struct Vector4Int : IEquatable<Vector4Int>
    {
        public int x, y, z, w;

        public Vector4Int(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static Vector4Int operator +(Vector4Int a, Vector4Int b)
        {
            return new Vector4Int(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public bool Equals(Vector4Int other)
        {
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector4Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine
            {
                int hash = 17;
                hash = hash * 31 + x;
                hash = hash * 31 + y;
                hash = hash * 31 + z;
                hash = hash * 31 + w;
                return hash;
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            LeniaDataGenerator4D generator = new LeniaDataGenerator4D();

            // Parse command-line arguments
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    string param = args[i].Substring(2);
                    if (i + 1 < args.Length)
                    {
                        string value = args[i + 1];
                        i++;
                        switch (param)
                        {
                            case "numFrames": generator.numFrames = int.Parse(value); break;
                            case "kernelRadius": generator.kernelRadius = int.Parse(value); break;
                            case "kernelSigmaMultiplier": generator.kernelSigmaMultiplier = float.Parse(value); break;
                            case "growthSigmaMultiplier": generator.growthSigmaMultiplier = float.Parse(value); break;
                            case "center": generator.center = float.Parse(value); break;
                            case "deltaT": generator.deltaT = float.Parse(value); break;
                            case "startingAreaSize": generator.startingAreaSize = int.Parse(value); break;
                            case "cellSpawnChance": generator.cellSpawnChance = float.Parse(value); break;
                            case "minInitialValue": generator.minInitialValue = float.Parse(value); break;
                            case "maxInitialValue": generator.maxInitialValue = float.Parse(value); break;
                            case "outputDirectory": generator.outputDirectory = value; break;
                            case "maxFrameTimeSeconds": generator.maxFrameTimeSeconds = float.Parse(value); break;
                            case "maxCellMass": generator.maxCellMass = float.Parse(value); break;
                            case "startingPoints": generator.startingPoints = int.Parse(value); break;
                            case "randomOffsetRange": generator.randomOffsetRange = int.Parse(value); break;
                            default: Console.WriteLine($"Unknown parameter: {param}"); break;
                        }
                    }
                }
            }

            generator.Run();
        }
    }

    public class LeniaDataGenerator4D
    {
        public int numFrames = 3;
        public int kernelRadius = 2;
        public float kernelSigmaMultiplier = 0.125f;
        public float growthSigmaMultiplier = 0.125f;
        public float center = 0.15f;
        public float deltaT = 1f;
        public int startingAreaSize = 30;
        public float cellSpawnChance = 0.1f;
        public float minInitialValue = 0.1f;
        public float maxInitialValue = 1.0f;
        public string outputDirectory = "LeniaData4D";
        public float maxFrameTimeSeconds = 1500.0f;
        public int startingPoints = 1;
        public int randomOffsetRange = 0;
        public float maxCellMass = float.MaxValue;

        private float kernelSigma;
        private float growthSigma;
        private Dictionary<Vector4Int, float> aliveCells = new Dictionary<Vector4Int, float>();
        private float[] kernelValues;
        private List<Vector4Int> kernelOffsets = new List<Vector4Int>();
        private Random random = new Random();

        public class FrameData
        {
            public CellData[] cells { get; set; }
        }

        public class CellData
        {
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }
            public int w { get; set; }
            public float value { get; set; }
        }

        public void Run()
        {
            kernelSigma = kernelRadius * kernelSigmaMultiplier;
            growthSigma = kernelRadius * growthSigmaMultiplier;
            InitializeKernel();
            InitializeGame();
            PrecomputeFrames();
        }

        void InitializeKernel()
        {
            float kernelSigmaSquared = kernelSigma * kernelSigma;
            List<float> kernelValueList = new List<float>();
            float r0 = kernelRadius / 2.0f;
            float sum = 0.0f;

            Parallel.For(-kernelRadius, kernelRadius + 1, i =>
            {
                for (int j = -kernelRadius; j <= kernelRadius; j++)
                for (int k = -kernelRadius; k <= kernelRadius; k++)
                for (int l = -kernelRadius; l <= kernelRadius; l++)
                {
                    float r = (float)Math.Sqrt(i * i + j * j + k * k + l * l);
                    float exponent = -(float)Math.Pow((r - r0), 2) / (2 * kernelSigmaSquared);
                    float value = (float)Math.Exp(exponent);

                    lock (kernelOffsets)
                    {
                        kernelOffsets.Add(new Vector4Int(i, j, k, l));
                        kernelValueList.Add(value);
                    }
                    sum += value;
                }
            });

            // Normalize kernel values
            kernelValues = new float[kernelValueList.Count];
            for (int idx = 0; idx < kernelValueList.Count; idx++)
            {
                kernelValues[idx] = kernelValueList[idx] / sum;
            }
        }

        void InitializeGame()
        {
            int halfSize = startingAreaSize / 2;
            int halfOffsetRange = randomOffsetRange / 2;

            // Create multiple starting points
            for (int point = 0; point < startingPoints; point++)
            {
                // Generate random offset for this starting point
                int offsetX = random.Next(-halfOffsetRange, halfOffsetRange + 1);
                int offsetY = random.Next(-halfOffsetRange, halfOffsetRange + 1);
                int offsetZ = random.Next(-halfOffsetRange, halfOffsetRange + 1);
                int offsetW = random.Next(-halfOffsetRange, halfOffsetRange + 1);

                for (int x = -halfSize; x < halfSize; x++)
                {
                    for (int y = -halfSize; y < halfSize; y++)
                    {
                        for (int z = -halfSize; z < halfSize; z++)
                        {
                            for (int w = -halfSize; w < halfSize; w++)
                            {
                                if (random.NextDouble() < cellSpawnChance)
                                {
                                    Vector4Int position = new Vector4Int(x + offsetX, y + offsetY, z + offsetZ, w + offsetW);
                                    float initialValue = (float)(random.NextDouble() * (maxInitialValue - minInitialValue) + minInitialValue);
                                    aliveCells[position] = initialValue;
                                }
                            }
                        }
                    }
                }
            }
        }

        void PrecomputeFrames()
        {
            string baseOutputPath = outputDirectory;
            string behavior = "";
            
            string fullOutputPath = Path.GetFullPath(outputDirectory);
            Directory.CreateDirectory(fullOutputPath);
            
            var sw = new Stopwatch();
            int actualFrameCount = 0;

            int targetFrameThreshold = (int)(numFrames * 0.6);
            bool reachedThreshold = false;
            
            for (int i = 0; i < numFrames; i++)
            {
                sw.Restart();
                
                float totalMass = aliveCells.Values.Sum();
                if (totalMass > maxCellMass)
                {
                    behavior = "timed_out";
                    break;
                }
                
                SaveFrameToFile(i, aliveCells);
                NextGeneration();
                
                actualFrameCount = i + 1;  // Track actual number of frames
                
                sw.Stop();
                
                if (sw.Elapsed.TotalSeconds > maxFrameTimeSeconds)
                {
                    behavior = "timed_out";
                    break;
                }

                if (i >= targetFrameThreshold && aliveCells.Count > 0)
                {
                    reachedThreshold = true;
                }

                if (aliveCells.Count == 0)
                {
                    behavior = reachedThreshold ? "lived" : "died";
                    break;
                }

                Console.WriteLine($"Frame {i + 1}/{numFrames} rendered in {sw.Elapsed.TotalSeconds:F2} seconds.");
            }

            if (string.IsNullOrEmpty(behavior))
            {
                behavior = "unstable";
            }

            string parentDir = Path.GetDirectoryName(Path.GetFullPath(baseOutputPath));
            string behaviorPath = Path.Combine(parentDir, behavior);
            Directory.CreateDirectory(behaviorPath);

            // Modify the simulation name to include actual frame count
            string simName = Path.GetFileName(baseOutputPath);
            simName = $"FRMS{actualFrameCount}_{simName}";
            string newFullOutputPath = Path.Combine(behaviorPath, simName);
            
            // Handle duplicate names
            int version = 1;
            string baseSimName = simName;
            while (Directory.Exists(newFullOutputPath))
            {
                version++;
                simName = $"{baseSimName}_v{version}";
                newFullOutputPath = Path.Combine(behaviorPath, simName);
            }
            
            Directory.Move(fullOutputPath, newFullOutputPath);
            outputDirectory = newFullOutputPath;

            Console.WriteLine($"4D Lenia simulation completed with behavior: {behavior}");
            Console.WriteLine($"Output saved to: {newFullOutputPath}");
        }

        async Task SaveFrameToFile(int frameIndex, Dictionary<Vector4Int, float> frameData)
        {
            string fullOutputPath = Path.Combine(Directory.GetCurrentDirectory(), outputDirectory);
            string filePath = Path.Combine(fullOutputPath, $"frame_{frameIndex}.json");

            List<CellData> cells = new List<CellData>();
            foreach (var kvp in frameData)
            {
                cells.Add(new CellData
                {
                    x = kvp.Key.x,
                    y = kvp.Key.y,
                    z = kvp.Key.z,
                    w = kvp.Key.w,
                    value = kvp.Value
                });
            }

            FrameData frame = new FrameData { cells = cells.ToArray() };

            var options = new JsonSerializerOptions { WriteIndented = false };
            string json = JsonSerializer.Serialize(frame, options);
            await File.WriteAllTextAsync(filePath, json);
        }

        public void NextGeneration()
        {
            Dictionary<Vector4Int, float> newAliveCells = new Dictionary<Vector4Int, float>();
            HashSet<Vector4Int> positionsToUpdate = new HashSet<Vector4Int>();

            // Calculate the current bounds
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            int minZ = int.MaxValue, maxZ = int.MinValue;
            int minW = int.MaxValue, maxW = int.MinValue;

            // First, collect ALL positions that need to be checked and calculate bounds
            foreach (var cellPos in aliveCells.Keys)
            {
                // Update bounds for each dimension
                minX = Math.Min(minX, cellPos.x - kernelRadius);
                maxX = Math.Max(maxX, cellPos.x + kernelRadius);
                minY = Math.Min(minY, cellPos.y - kernelRadius);
                maxY = Math.Max(maxY, cellPos.y + kernelRadius);
                minZ = Math.Min(minZ, cellPos.z - kernelRadius);
                maxZ = Math.Max(maxZ, cellPos.z + kernelRadius);
                minW = Math.Min(minW, cellPos.w - kernelRadius);
                maxW = Math.Max(maxW, cellPos.w + kernelRadius);
            }

            // Check ALL positions within the bounds plus kernel radius
            for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            for (int z = minZ; z <= maxZ; z++)
            for (int w = minW; w <= maxW; w++)
            {
                Vector4Int position = new Vector4Int(x, y, z, w);
                float convolutionValue = CalculateConvolution(position);
                float growth = GrowthFunction(convolutionValue);
                
                float currentValue = aliveCells.TryGetValue(position, out float value) ? value : 0f;
                float newValue = Clamp01(currentValue + deltaT * (2 * growth - 1));

                if (newValue > 0.001f)
                {
                    newAliveCells[position] = newValue;
                }
            }

            // Debug output to verify growth
            Console.WriteLine($"Cells changed from {aliveCells.Count} to {newAliveCells.Count}");
            Console.WriteLine($"Bounds: X({minX} to {maxX}), Y({minY} to {maxY}), Z({minZ} to {maxZ}), W({minW} to {maxW})");

            aliveCells = newAliveCells;
        }

        private float CalculateConvolution(Vector4Int position)
        {
            float sum = 0f;
            for (int idx = 0; idx < kernelOffsets.Count; idx++)
            {
                Vector4Int neighborPos = position + kernelOffsets[idx];
                if (aliveCells.TryGetValue(neighborPos, out float neighborValue))
                {
                    sum += neighborValue * kernelValues[idx];
                }
            }
            return sum;
        }

        float GrowthFunction(float x)
        {
            float exponent = -((x - center) * (x - center)) / (2 * growthSigma * growthSigma);
            return (float)Math.Exp(exponent);
        }

        float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }
}
