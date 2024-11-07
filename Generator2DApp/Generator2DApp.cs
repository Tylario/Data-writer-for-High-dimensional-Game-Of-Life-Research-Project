using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Generator2D
{
    public struct Vector2Int
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
                            case "startingAreaSize": generator.startingAreaSize = int.Parse(value); break;
                            case "minInitialValue": generator.minInitialValue = float.Parse(value); break;
                            case "maxInitialValue": generator.maxInitialValue = float.Parse(value); break;
                            case "cellSpawnChance": generator.cellSpawnChance = float.Parse(value); break;
                            case "deltaT": generator.deltaT = float.Parse(value); break;
                            case "center": generator.center = float.Parse(value); break;
                            case "sigma": generator.sigma = float.Parse(value); break;
                            case "outputDirectory": generator.outputDirectory = value; break;
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
        public float sigma = 0.012f;
        public string outputDirectory = "LeniaData2D";

        private Dictionary<Vector2Int, float> aliveCells = new Dictionary<Vector2Int, float>();
        private List<Vector2Int> kernelOffsets = new List<Vector2Int>();
        private float[] kernelValues;
        private Random random = new Random();

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
            InitializeKernel();
            InitializeGame();
            PrecomputeFrames();
        }

        void InitializeKernel()
        {
            List<float> kernelValueList = new List<float>();
            float r0 = kernelRadius / 2.0f;
            float sigmaK = kernelRadius / 4.0f;
            float sum = 0.0f;

            for (int i = -kernelRadius; i <= kernelRadius; i++)
            {
                for (int j = -kernelRadius; j <= kernelRadius; j++)
                {
                    float r = (float)Math.Sqrt(i * i + j * j);
                    float value = (float)Math.Exp(-((r - r0) * (r - r0)) / (2 * sigmaK * sigmaK));
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
                Console.WriteLine($"Frame {i + 1}/{numFrames} rendered.");
            }

            Console.WriteLine("Precomputing done.");
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
            Dictionary<Vector2Int, float> newAliveCells = new Dictionary<Vector2Int, float>();
            HashSet<Vector2Int> positionsToUpdate = new HashSet<Vector2Int>();

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

        private float CalculateConvolution(Vector2Int position)
        {
            float sum = 0f;

            for (int idx = 0; idx < kernelOffsets.Count; idx++)
            {
                Vector2Int neighborPos = position + kernelOffsets[idx];
                if (aliveCells.TryGetValue(neighborPos, out float neighborValue))
                {
                    sum += neighborValue * kernelValues[idx];
                }
            }

            return sum;
        }

        float GrowthFunction(float x)
        {
            float exponent = -((x - center) * (x - center)) / (2 * sigma * sigma);
            return (float)Math.Exp(exponent);
        }

        float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }
}
