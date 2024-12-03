using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Generator2D
{
    public struct Vector2Int : IEquatable<Vector2Int>
    {
        public int x, y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2Int other && Equals(other);
        }

        public bool Equals(Vector2Int other)
        {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            unchecked // Allow overflow
            {
                int hash = 17;
                hash = hash * 31 + x;
                hash = hash * 31 + y;
                return hash;
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            LeniaDataGenerator2D generator = new LeniaDataGenerator2D();

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
                            case "startingAreaSize": generator.startingAreaSize = int.Parse(value); break;
                            case "minInitialValue": generator.minInitialValue = float.Parse(value); break;
                            case "maxInitialValue": generator.maxInitialValue = float.Parse(value); break;
                            case "cellSpawnChance": generator.cellSpawnChance = float.Parse(value); break;
                            case "deltaT": generator.deltaT = float.Parse(value); break;
                            case "center": generator.center = float.Parse(value); break;
                            case "outputDirectory": generator.outputDirectory = value; break;
                            case "maxFrameTimeSeconds": generator.maxFrameTimeSeconds = float.Parse(value); break;
                            case "growthSteepness": generator.growthSteepness = float.Parse(value); break;
                                default: Console.WriteLine($"Unknown parameter: {param}"); break;
                        }
                    }
                }
            }

            generator.Run();
        }
    }

    public class LeniaDataGenerator2D
    {
        public int numFrames = 100;
        public int kernelRadius = 14;
        public int startingAreaSize = 50;
        public float minInitialValue = 0.1f;
        public float maxInitialValue = 1.0f;
        public float cellSpawnChance = 0.4f;
        public float deltaT = 0.1f;
        public float center = 0.15f;
        public float kernelSigmaMultiplier = 0.125f;
        public float growthSigmaMultiplier = 0.125f;
        public string outputDirectory = "LeniaData2D";
        public float maxFrameTimeSeconds = 1500.0f;
        public float growthSteepness = 1.0f;

        private Dictionary<Vector2Int, float> aliveCells = new Dictionary<Vector2Int, float>();
        private List<Vector2Int> kernelOffsets = new List<Vector2Int>();
        private float[] kernelValues;
        private Random random = new Random();

        private float kernelSigma;
        private float growthSigma;

        public class FrameData
        {
            public CellData[] cells { get; set; }
        }

        public class CellData
        {
            public int x { get; set; }
            public int y { get; set; }
            public float value { get; set; }
        }

        public void Run()
        {
            // Calculate kernel and growth sigmas
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
            {
                for (int j = -kernelRadius; j <= kernelRadius; j++)
                {
                    float r = (float)Math.Sqrt(i * i + j * j);
                    float exponent = -((r - r0) * (r - r0)) / (2 * kernelSigma * kernelSigma);
                    float value = (float)Math.Exp(exponent);
                    kernelOffsets.Add(new Vector2Int(i, j));
                    kernelValueList.Add(value);
                    sum += value;
                }
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
            {
                for (int y = -halfSize; y < halfSize; y++)
                {
                    if (random.NextDouble() < cellSpawnChance)
                    {
                        Vector2Int position = new Vector2Int(x, y);
                        float initialValue = (float)(random.NextDouble() * (maxInitialValue - minInitialValue) + minInitialValue);
                        aliveCells[position] = initialValue;
                    }
                }
            }
        }

        void PrecomputeFrames()
        {
            string baseOutputPath = outputDirectory;
            string endBehavior = "";
            
            // Create base directory first
            string fullOutputPath = Path.Combine(Directory.GetCurrentDirectory(), outputDirectory);
            Directory.CreateDirectory(fullOutputPath);
            
            var sw = new Stopwatch();

            for (int i = 0; i < numFrames; i++)
            {
                sw.Restart();
                
                SaveFrameToFile(i, aliveCells);
                NextGeneration();
                
                sw.Stop();
                
                // Check if frame took too long
                if (sw.Elapsed.TotalSeconds > maxFrameTimeSeconds)
                {
                    endBehavior = "_stable";
                    Console.WriteLine($"Frame {i} took {sw.Elapsed.TotalSeconds:F2} seconds, exceeding limit of {maxFrameTimeSeconds} seconds.");
                    break;
                }

                // Check if simulation died
                if (aliveCells.Count == 0)
                {
                    endBehavior = "_dead";
                    Console.WriteLine("No cells remaining, simulation died.");
                    break;
                }

                Console.WriteLine($"Frame {i + 1}/{numFrames} rendered in {sw.Elapsed.TotalSeconds:F2} seconds.");
            }

            // If we completed all frames without exploding or dying, it's stable
            if (string.IsNullOrEmpty(endBehavior))
            {
                endBehavior = "_stable";
            }

            // Rename the directory with the end behavior
            string newOutputPath = baseOutputPath + endBehavior;
            string newFullOutputPath = Path.Combine(Directory.GetCurrentDirectory(), newOutputPath);
            
            // If the new path already exists, delete it
            if (Directory.Exists(newFullOutputPath))
            {
                Directory.Delete(newFullOutputPath, true);
            }
            
            // Rename the directory
            Directory.Move(fullOutputPath, newFullOutputPath);
            outputDirectory = newOutputPath;

            Console.WriteLine($"2D Lenia simulation completed with behavior: {endBehavior.Substring(1)}");
        }

        void SaveFrameToFile(int frameIndex, Dictionary<Vector2Int, float> frameData)
        {
            string fullOutputPath = Path.Combine(Directory.GetCurrentDirectory(), outputDirectory);
            string filePath = Path.Combine(fullOutputPath, $"frame_{frameIndex}.json");

            CellData[] cells = new CellData[frameData.Count];
            int index = 0;
            foreach (var kvp in frameData)
            {
                cells[index++] = new CellData
                {
                    x = kvp.Key.x,
                    y = kvp.Key.y,
                    value = kvp.Value
                };
            }

            FrameData frame = new FrameData { cells = cells };
            var options = new JsonSerializerOptions { WriteIndented = false };
            string json = JsonSerializer.Serialize(frame, options);
            File.WriteAllText(filePath, json);
        }

        public void NextGeneration()
        {
            var convolutionValues = new ConcurrentDictionary<Vector2Int, float>();

            // Compute convolution values by scattering contributions from alive cells
            Parallel.ForEach(aliveCells, aliveCellKvp =>
            {
                var aliveCellPos = aliveCellKvp.Key;
                var aliveCellValue = aliveCellKvp.Value;

                for (int idx = 0; idx < kernelOffsets.Count; idx++)
                {
                    Vector2Int neighborPos = aliveCellPos + kernelOffsets[idx];
                    float kernelValue = kernelValues[idx];
                    float contribution = aliveCellValue * kernelValue;

                    // Update convolution value atomically
                    convolutionValues.AddOrUpdate(neighborPos, contribution, (key, oldValue) => oldValue + contribution);
                }
            });

            var newAliveCells = new ConcurrentDictionary<Vector2Int, float>();

            // Compute new cell values based on convolution values
            Parallel.ForEach(convolutionValues, convolutionKvp =>
            {
                Vector2Int position = convolutionKvp.Key;
                float convolutionValue = convolutionKvp.Value;

                float growth = GrowthFunction(convolutionValue);

                float currentValue = aliveCells.TryGetValue(position, out float value) ? value : 0f;
                float newValue = Clamp01(currentValue + deltaT * (2 * growth - 1));

                if (newValue > 0.01f)
                {
                    newAliveCells[position] = newValue;
                }
            });

            aliveCells = new Dictionary<Vector2Int, float>(newAliveCells);
        }

        float GrowthFunction(float x)
        {
            float exponent = -growthSteepness * ((x - center) * (x - center)) / (2 * growthSigma * growthSigma);
            return (float)Math.Exp(exponent);
        }

        float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }
}
