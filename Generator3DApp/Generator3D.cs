using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

namespace Generator3D
{
    public struct Vector3Int : IEquatable<Vector3Int>
    {
        public int x, y, z;

        public Vector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3Int operator +(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public bool Equals(Vector3Int other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine
            {
                int hash = 17;
                hash = hash * 31 + x;
                hash = hash * 31 + y;
                hash = hash * 31 + z;
                return hash;
            }
        }
    }


    public class Program
    {
        static void Main(string[] args)
        {
            LeniaDataGenerator3D generator = new LeniaDataGenerator3D();

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

    public class LeniaDataGenerator3D
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
        public string outputDirectory = "LeniaData3D";
        public float maxFrameTimeSeconds = 1500.0f;
        public int startingPoints = 1;
        public int randomOffsetRange = 0;
        public float maxCellMass = float.MaxValue;

        private float kernelSigma;
        private float growthSigma;
        private Dictionary<Vector3Int, float> aliveCells = new Dictionary<Vector3Int, float>();
        private float[] kernelValues;
        private List<Vector3Int> kernelOffsets = new List<Vector3Int>();
        private Random random = new Random();

        // Add new fields for optimization
        private float kernelR0Squared;
        private float kernelSigmaSquared;
        private float growthSigmaSquared;
        private float twoGrowthSigmaSquared;
        private Dictionary<Vector3Int, int> kernelOffsetIndices;

        public class FrameData
        {
            public CellData[] cells { get; set; }
        }

        public class CellData
        {
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }
            public float value { get; set; }
        }

        public void Run()
        {
            kernelSigma = kernelRadius * kernelSigmaMultiplier;
            growthSigma = kernelRadius * growthSigmaMultiplier;
            
            // Precompute squared values
            kernelR0Squared = (float)Math.Pow(kernelRadius / 2.0f, 2);
            kernelSigmaSquared = kernelSigma * kernelSigma;
            growthSigmaSquared = growthSigma * growthSigma;
            twoGrowthSigmaSquared = 2 * growthSigmaSquared;
            
            InitializeKernel();
            InitializeGame();
            PrecomputeFrames();
        }

        void InitializeKernel()
        {
            List<float> kernelValueList = new List<float>();
            kernelOffsetIndices = new Dictionary<Vector3Int, int>();
            float sum = 0.0f;
            int index = 0;

            for (int i = -kernelRadius; i <= kernelRadius; i++)
            {
                for (int j = -kernelRadius; j <= kernelRadius; j++)
                {
                    for (int k = -kernelRadius; k <= kernelRadius; k++)
                    {
                        float rSquared = i * i + j * j + k * k;
                        float exponent = -(rSquared - kernelR0Squared) / (2 * kernelSigmaSquared);
                        float value = (float)Math.Exp(exponent);

                        Vector3Int offset = new Vector3Int(i, j, k);
                        kernelOffsets.Add(offset);
                        kernelOffsetIndices[offset] = index++;
                        kernelValueList.Add(value);
                        sum += value;
                    }
                }
            }

            // Normalize kernel values
            kernelValues = kernelValueList.ToArray();
            float invSum = 1f / sum;
            for (int idx = 0; idx < kernelValues.Length; idx++)
            {
                kernelValues[idx] *= invSum;
            }
        }

        void InitializeGame()
        {
            int halfSize = startingAreaSize / 2;
            int halfOffsetRange = randomOffsetRange / 2;

            // Create multiple starting points
            for (int point = 0; point < startingPoints; point++)
            {
                // Generate random offset for this starting point, but keep x=0
                int offsetX = 0; // Force x offset to be 0
                int offsetY = random.Next(-halfOffsetRange, halfOffsetRange + 1);
                int offsetZ = random.Next(-halfOffsetRange, halfOffsetRange + 1);

                for (int x = -halfSize; x < halfSize; x++)
                {
                    for (int y = -halfSize; y < halfSize; y++)
                    {
                        for (int z = -halfSize; z < halfSize; z++)
                        {
                            if (random.NextDouble() < cellSpawnChance)
                            {
                                Vector3Int position = new Vector3Int(x + offsetX, y + offsetY, z + offsetZ);
                                float initialValue = (float)(random.NextDouble() * (maxInitialValue - minInitialValue) + minInitialValue);
                                aliveCells[position] = initialValue;
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

                if (aliveCells.Count == 0)
                {
                    behavior = i < 25 ? "died" : "lived";
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

            Console.WriteLine($"3D Lenia simulation completed with behavior: {behavior}");
            Console.WriteLine($"Output saved to: {newFullOutputPath}");
        }

        void SaveFrameToFile(int frameIndex, Dictionary<Vector3Int, float> frameData)
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
                    value = kvp.Value
                });
            }

            FrameData frame = new FrameData { cells = cells.ToArray() };

            var options = new JsonSerializerOptions { WriteIndented = false };
            string json = JsonSerializer.Serialize(frame, options);
            File.WriteAllText(filePath, json);
        }

        public void NextGeneration()
        {
            var convolutionValues = new ConcurrentDictionary<Vector3Int, float>();

            // Compute convolution values by scattering contributions from alive cells
            Parallel.ForEach(aliveCells, aliveCellKvp =>
            {
                var aliveCellPos = aliveCellKvp.Key;
                var aliveCellValue = aliveCellKvp.Value;

                for (int idx = 0; idx < kernelOffsets.Count; idx++)
                {
                    Vector3Int neighborPos = aliveCellPos + kernelOffsets[idx];
                    float kernelValue = kernelValues[idx];
                    float contribution = aliveCellValue * kernelValue;

                    // Update convolution value atomically
                    convolutionValues.AddOrUpdate(neighborPos, contribution, (key, oldValue) => oldValue + contribution);
                }
            });

            var newAliveCells = new ConcurrentDictionary<Vector3Int, float>();

            // Compute new cell values based on convolution values
            Parallel.ForEach(convolutionValues, convolutionKvp =>
            {
                Vector3Int position = convolutionKvp.Key;
                float convolutionValue = convolutionKvp.Value;

                float growth = GrowthFunction(convolutionValue);

                float currentValue = aliveCells.TryGetValue(position, out float value) ? value : 0f;
                float newValue = Clamp01(currentValue + deltaT * (2 * growth - 1));

                if (newValue > 0.01f)
                {
                    newAliveCells[position] = newValue;
                }
            });

            aliveCells = new Dictionary<Vector3Int, float>(newAliveCells);
        }

        float GrowthFunction(float x)
        {
            float diff = x - center;
            return (float)Math.Exp(-(diff * diff) / twoGrowthSigmaSquared);
        }

        float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }
}
