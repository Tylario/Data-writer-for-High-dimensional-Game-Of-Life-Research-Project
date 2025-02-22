﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

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
                            case "startingPoints": generator.startingPoints = int.Parse(value); break;
                            case "randomOffsetRange": generator.randomOffsetRange = int.Parse(value); break;
                            case "maxCellMass": generator.maxCellMass = float.Parse(value); break;
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
        public int startingPoints = 1;
        public int randomOffsetRange = 0;
        public float maxCellMass = float.MaxValue;

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
            int halfOffsetRange = randomOffsetRange / 2;

            // Create multiple starting points
            for (int point = 0; point < startingPoints; point++)
            {
                // Generate random offset for this starting point
                int offsetX = random.Next(-halfOffsetRange, halfOffsetRange + 1);
                int offsetY = random.Next(-halfOffsetRange, halfOffsetRange + 1);

                for (int x = -halfSize; x < halfSize; x++)
                {
                    for (int y = -halfSize; y < halfSize; y++)
                    {
                        if (random.NextDouble() < cellSpawnChance)
                        {
                            Vector2Int position = new Vector2Int(x + offsetX, y + offsetY);
                            float initialValue = (float)(random.NextDouble() * (maxInitialValue - minInitialValue) + minInitialValue);
                            aliveCells[position] = initialValue;
                        }
                    }
                }
            }
        }

        void PrecomputeFrames()
        {
            string baseOutputPath = outputDirectory;
            string behavior = "";
            
            // Create base directory first
            string fullOutputPath = Path.GetFullPath(outputDirectory);
            Directory.CreateDirectory(fullOutputPath);
            
            var sw = new Stopwatch();

            int targetFrameThreshold = 100;
            bool reachedThreshold = false;
            
            int actualFrameCount = 0;
            
            for (int i = 0; i < numFrames; i++)
            {
                sw.Restart();
                
                // Calculate total mass
                float totalMass = aliveCells.Values.Sum();
                if (totalMass > maxCellMass)
                {
                    behavior = "timed_out";
                    Console.WriteLine($"Total mass {totalMass:F2} exceeded limit of {maxCellMass:F2}.");
                    break;
                }
                
                SaveFrameToFile(i, aliveCells);
                NextGeneration();
                
                actualFrameCount = i + 1;  // Track actual number of frames
                
                sw.Stop();
                
                // Check if frame took too long
                if (sw.Elapsed.TotalSeconds > maxFrameTimeSeconds)
                {
                    behavior = "timed_out";
                    Console.WriteLine($"Frame {i} took {sw.Elapsed.TotalSeconds:F2} seconds, exceeding limit of {maxFrameTimeSeconds} seconds.");
                    break;
                }

                // Check if we've reached the threshold for "lived" status
                if (i >= targetFrameThreshold && aliveCells.Count > 0)
                {
                    reachedThreshold = true;
                }

                // Check if simulation died
                if (aliveCells.Count == 0)
                {
                    behavior = reachedThreshold ? "lived" : "died";
                    Console.WriteLine($"No cells remaining after {i} frames.");
                    break;
                }

                Console.WriteLine($"Frame {i + 1}/{numFrames} rendered in {sw.Elapsed.TotalSeconds:F2} seconds.");
            }

            // If we completed all frames without timing out or dying, it's unstable
            if (string.IsNullOrEmpty(behavior))
            {
                behavior = "unstable";
            }

            // Create the parent directory of the output directory if it doesn't exist
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

            Console.WriteLine($"2D Lenia simulation completed with behavior: {behavior}");
            Console.WriteLine($"Output saved to: {newFullOutputPath}");
        }

        void SaveFrameToFile(int frameIndex, Dictionary<Vector2Int, float> frameData)
        {
            string fullOutputPath = Path.GetFullPath(outputDirectory);
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
