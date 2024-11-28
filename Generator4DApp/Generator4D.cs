using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;

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
            List<float> kernelValueList = new List<float>();
            float r0 = kernelRadius / 2.0f;
            float sum = 0.0f;

            for (int i = -kernelRadius; i <= kernelRadius; i++)
            for (int j = -kernelRadius; j <= kernelRadius; j++)
            for (int k = -kernelRadius; k <= kernelRadius; k++)
            for (int l = -kernelRadius; l <= kernelRadius; l++)
            {
                float r = (float)Math.Sqrt(i * i + j * j + k * k + l * l);
                float exponent = -(float)Math.Pow((r - r0), 2) / (2 * (float)Math.Pow(kernelSigma, 2));
                float value = (float)Math.Exp(exponent);

                kernelOffsets.Add(new Vector4Int(i, j, k, l));
                kernelValueList.Add(value);
                sum += value;
            }

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

            for (int x = -halfSize; x < halfSize; x++)
            for (int y = -halfSize; y < halfSize; y++)
            for (int z = -halfSize; z < halfSize; z++)
            for (int w = -halfSize; w < halfSize; w++)
            {
                if (random.NextDouble() < cellSpawnChance)
                {
                    Vector4Int position = new Vector4Int(x, y, z, w);
                    float initialValue = (float)(random.NextDouble() * (maxInitialValue - minInitialValue) + minInitialValue);
                    aliveCells[position] = initialValue;
                }
            }
        }

        void PrecomputeFrames()
        {
            string baseOutputPath = outputDirectory;
            string endBehavior = "";
            
            string fullOutputPath = Path.Combine(Directory.GetCurrentDirectory(), outputDirectory);
            Directory.CreateDirectory(fullOutputPath);
            
            var sw = new Stopwatch();

            for (int i = 0; i < numFrames; i++)
            {
                Console.WriteLine($"Rendering Frame {i + 1}/{numFrames}...");
                
                sw.Restart();
                
                SaveFrameToFile(i, aliveCells);
                NextGeneration();
                
                sw.Stop();
                
                if (sw.Elapsed.TotalSeconds > maxFrameTimeSeconds)
                {
                    endBehavior = "_exploded";
                    Console.WriteLine($"Frame {i} took {sw.Elapsed.TotalSeconds:F2} seconds, exceeding limit of {maxFrameTimeSeconds} seconds.");
                    break;
                }

                if (aliveCells.Count == 0)
                {
                    endBehavior = "_dead";
                    Console.WriteLine("No cells remaining, simulation died.");
                    break;
                }

                Console.WriteLine($"Frame {i + 1}/{numFrames} rendered in {sw.Elapsed.TotalSeconds:F2} seconds.");
            }

            if (string.IsNullOrEmpty(endBehavior))
            {
                endBehavior = "_unstable";
            }

            string newOutputPath = baseOutputPath + endBehavior;
            string newFullOutputPath = Path.Combine(Directory.GetCurrentDirectory(), newOutputPath);
            
            if (Directory.Exists(newFullOutputPath))
            {
                Directory.Delete(newFullOutputPath, true);
            }
            
            Directory.Move(fullOutputPath, newFullOutputPath);
            outputDirectory = newOutputPath;

            Console.WriteLine($"4D Lenia simulation completed with behavior: {endBehavior.Substring(1)}");
        }

        void SaveFrameToFile(int frameIndex, Dictionary<Vector4Int, float> frameData)
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
            File.WriteAllText(filePath, json);
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
