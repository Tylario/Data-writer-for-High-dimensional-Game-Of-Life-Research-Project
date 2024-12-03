using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Diagnostics;

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

        private float kernelSigma;
        private float growthSigma;
        private Dictionary<Vector3Int, float> aliveCells = new Dictionary<Vector3Int, float>();
        private float[] kernelValues;
        private List<Vector3Int> kernelOffsets = new List<Vector3Int>();
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
                        float r = (float)Math.Sqrt(i * i + j * j + k * k);
                        float exponent = -(float)Math.Pow((r - r0), 2) / (2 * (float)Math.Pow(kernelSigma, 2));
                        float value = (float)Math.Exp(exponent);

                        kernelOffsets.Add(new Vector3Int(i, j, k));
                        kernelValueList.Add(value);
                        sum += value;
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
                        if (random.NextDouble() < cellSpawnChance)
                        {
                            Vector3Int position = new Vector3Int(x, y, z);
                            float initialValue = (float)(random.NextDouble() * (maxInitialValue - minInitialValue) + minInitialValue);
                            aliveCells[position] = initialValue;
                        }
                    }
                }
            }
        }

        void PrecomputeFrames()
        {
            // Create base simulation directory structure
            string baseSimPath = Path.Combine(Directory.GetCurrentDirectory(), "Simulations");
            string dim3DPath = Path.Combine(baseSimPath, "3D");
            string stablePath = Path.Combine(dim3DPath, "stable");
            string deadPath = Path.Combine(dim3DPath, "dead");
            string unstablePath = Path.Combine(dim3DPath, "unstable");

            Directory.CreateDirectory(baseSimPath);
            Directory.CreateDirectory(dim3DPath);
            Directory.CreateDirectory(stablePath);
            Directory.CreateDirectory(deadPath);
            Directory.CreateDirectory(unstablePath);

            // Create temporary working directory
            string tempDir = Path.Combine(Directory.GetCurrentDirectory(), outputDirectory);
            Directory.CreateDirectory(tempDir);

            var sw = new Stopwatch();
            string endState = "stable"; // Default state
            int previousCellCount = aliveCells.Count;
            int stableFrameCount = 0;

            for (int i = 0; i < numFrames; i++)
            {
                Console.WriteLine($"Rendering Frame {i + 1}/{numFrames}...");
                
                sw.Restart();
                SaveFrameToFile(i, aliveCells);
                NextGeneration();
                sw.Stop();

                // Check if frame took too long
                if (sw.Elapsed.TotalSeconds > maxFrameTimeSeconds)
                {
                    endState = "unstable";
                    Console.WriteLine($"Frame {i} took {sw.Elapsed.TotalSeconds:F2} seconds, exceeding limit of {maxFrameTimeSeconds} seconds.");
                    break;
                }

                // Check if simulation died
                if (aliveCells.Count == 0)
                {
                    endState = "dead";
                    Console.WriteLine("No cells remaining, simulation died.");
                    break;
                }

                // Check for stability
                if (Math.Abs(aliveCells.Count - previousCellCount) <= 5)
                {
                    stableFrameCount++;
                    if (stableFrameCount >= 10) // Consider stable if cell count remains similar for 10 frames
                    {
                        endState = "stable";
                    }
                }
                else
                {
                    stableFrameCount = 0;
                }

                previousCellCount = aliveCells.Count;
                Console.WriteLine($"Frame {i + 1}/{numFrames} rendered in {sw.Elapsed.TotalSeconds:F2} seconds.");
            }

            // Determine final destination based on end state
            string finalDestination = Path.Combine(dim3DPath, endState, Path.GetFileName(outputDirectory));

            // If the destination already exists, delete it
            if (Directory.Exists(finalDestination))
            {
                Directory.Delete(finalDestination, true);
            }

            // Move the temporary directory to final destination
            Directory.Move(tempDir, finalDestination);
            outputDirectory = finalDestination;

            Console.WriteLine($"3D Lenia simulation completed with state: {endState}");
            Console.WriteLine($"Output saved to: {finalDestination}");
        }

        void SaveFrameToFile(int frameIndex, Dictionary<Vector3Int, float> frameData)
        {
            string filePath = Path.Combine(outputDirectory, $"frame_{frameIndex}.json");

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
            Dictionary<Vector3Int, float> newAliveCells = new Dictionary<Vector3Int, float>();
            HashSet<Vector3Int> positionsToUpdate = new HashSet<Vector3Int>();

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

        private float CalculateConvolution(Vector3Int position)
        {
            float sum = 0f;
            for (int idx = 0; idx < kernelOffsets.Count; idx++)
            {
                Vector3Int neighborPos = position + kernelOffsets[idx];
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
