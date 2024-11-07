using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Generator4D
{
    // Custom struct to represent 4D integer vectors
    public struct Vector4Int
    {
        public int x, y, z, w;

        public Vector4Int(int x, int y, int z, int w)
        {
            this.x = x; this.y = y; this.z = z; this.w = w;
        }

        public static Vector4Int operator +(Vector4Int a, Vector4Int b)
        {
            return new Vector4Int(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        // Override Equals and GetHashCode for use in dictionaries
        public override bool Equals(object obj)
        {
            if (!(obj is Vector4Int))
                return false;
            Vector4Int other = (Vector4Int)obj;
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }

        public override int GetHashCode()
        {
            unchecked
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
            // Usage example: --numFrames 100 --outputDirectory MyData
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    string param = args[i].Substring(2);
                    if (i + 1 < args.Length)
                    {
                        string value = args[i + 1];
                        i++; // Move to next argument
                        switch (param)
                        {
                            case "numFrames":
                                generator.numFrames = int.Parse(value);
                                break;
                            case "kernelRadius":
                                generator.kernelRadius = int.Parse(value);
                                break;
                            case "kernelSigmaMultiplier":
                                generator.kernelSigmaMultiplier = float.Parse(value);
                                break;
                            case "growthSigmaMultiplier":
                                generator.growthSigmaMultiplier = float.Parse(value);
                                break;
                            case "center":
                                generator.center = float.Parse(value);
                                break;
                            case "deltaT":
                                generator.deltaT = float.Parse(value);
                                break;
                            case "startingAreaSize":
                                generator.startingAreaSize = int.Parse(value);
                                break;
                            case "cellSpawnChance":
                                generator.cellSpawnChance = float.Parse(value);
                                break;
                            case "minInitialValue":
                                generator.minInitialValue = float.Parse(value);
                                break;
                            case "maxInitialValue":
                                generator.maxInitialValue = float.Parse(value);
                                break;
                            case "outputDirectory":
                                generator.outputDirectory = value;
                                break;
                            default:
                                Console.WriteLine($"Unknown parameter: {param}");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Missing value for parameter: {param}");
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid argument: {args[i]}");
                }
            }

            generator.Run();
        }
    }

    public class LeniaDataGenerator4D
    {
        // Simulation parameters for 4D Lenia
        public int numFrames = 100;
        public int kernelRadius = 14;
        public float kernelSigmaMultiplier = 0.125f;
        public float growthSigmaMultiplier = 0.012f;
        public float center = 0.15f;
        public float deltaT = 0.1f;
        public int startingAreaSize = 50;
        public float cellSpawnChance = 0.4f;
        public float minInitialValue = 0.1f;
        public float maxInitialValue = 1.0f;
        public string outputDirectory = "LeniaData4D"; // Directory name where the generated frame data will be saved

        private float kernelSigma = 0;
        private float growthSigma = 0;
        private Dictionary<Vector4Int, float> aliveCells = new Dictionary<Vector4Int, float>();
        private float[] kernelValues;
        private List<Vector4Int> kernelOffsets = new List<Vector4Int>();
        private Random random = new Random();

        // Serializable classes for JSON
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
            {
                for (int j = -kernelRadius; j <= kernelRadius; j++)
                {
                    for (int k = -kernelRadius; k <= kernelRadius; k++)
                    {
                        for (int l = -kernelRadius; l <= kernelRadius; l++)
                        {
                            float r = (float)Math.Sqrt(i * i + j * j + k * k + l * l);
                            float exponent = -(float)Math.Pow((r - r0), 2) / (2 * (float)Math.Pow(kernelSigma, 2));
                            float value = (float)Math.Exp(exponent);

                            kernelOffsets.Add(new Vector4Int(i, j, k, l));
                            kernelValueList.Add(value);
                            sum += value;
                        }
                    }
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
                    for (int z = -halfSize; z < halfSize; z++)
                    {
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
                }
            }
        }

        void PrecomputeFrames()
        {
            string fullOutputPath = Path.Combine(Directory.GetCurrentDirectory(), outputDirectory);

            // Clear the output directory before starting
            if (Directory.Exists(fullOutputPath))
            {
                DirectoryInfo di = new DirectoryInfo(fullOutputPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(fullOutputPath);
            }

            for (int i = 0; i < numFrames; i++)
            {
                SaveFrameToFile(i, aliveCells);
                NextGeneration();
                Console.WriteLine($"Frame {i + 1} computed.");
            }

            Console.WriteLine("Precomputing done.");
        }

        void SaveFrameToFile(int frameIndex, Dictionary<Vector4Int, float> frameData)
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
                    z = kvp.Key.z,
                    w = kvp.Key.w,
                    value = kvp.Value
                };
            }

            FrameData frame = new FrameData { cells = cells };

            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = null, // Keep property names as they are
            };

            string json = JsonSerializer.Serialize(frame, options);
            File.WriteAllText(filePath, json);
        }

        public void NextGeneration()
        {
            Dictionary<Vector4Int, float> newAliveCells = new Dictionary<Vector4Int, float>();
            HashSet<Vector4Int> positionsToUpdate = new HashSet<Vector4Int>();

            // Collect positions to update
            foreach (var cellPos in aliveCells.Keys)
            {
                foreach (var offset in kernelOffsets)
                {
                    positionsToUpdate.Add(cellPos + offset);
                }
            }

            foreach (var position in positionsToUpdate)
            {
                float convolutionValue = CalculateConvolution(position);
                float growth = GrowthFunction(convolutionValue);

                float currentValue = aliveCells.TryGetValue(position, out float value) ? value : 0f;
                float newValue = Clamp01(currentValue + deltaT * (2 * growth - 1));

                if (newValue > 0.01f)
                {
                    newAliveCells[position] = newValue;
                }
            }

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
