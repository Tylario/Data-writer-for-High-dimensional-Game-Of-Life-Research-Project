using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

public class GeneratorManager
{
    private const string OUTPUTS_DIR = "outputs";
    private const string COOL_SIMULATIONS_DIR = "cool_simulations";
    private const string TWO_D_DIR = "2D";
    private const string THREE_D_DIR = "3D";
    private const string FOUR_D_DIR = "4D";

    // Define a class to hold simulation parameters
    public class Simulation
    {
        public string GeneratorPath { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;
    }

    // Lists to hold simulations
    private List<Simulation> simulations = new List<Simulation>();

    public GeneratorManager()
    {
        // Initialize directories
        EnsureDirectoryStructure();
        InitializeSimulations();
    }

    private void EnsureDirectoryStructure()
    {
        // Create main directories
        var directories = new[]
        {
            Path.Combine(OUTPUTS_DIR, TWO_D_DIR),
            Path.Combine(OUTPUTS_DIR, THREE_D_DIR),
            Path.Combine(OUTPUTS_DIR, FOUR_D_DIR),
            Path.Combine(COOL_SIMULATIONS_DIR, TWO_D_DIR),
            Path.Combine(COOL_SIMULATIONS_DIR, THREE_D_DIR),
            Path.Combine(COOL_SIMULATIONS_DIR, FOUR_D_DIR)
        };

        foreach (var dir in directories)
        {
            Directory.CreateDirectory(dir);
        }
    }

    private void InitializeSimulations()
    {
        // Define paths to each generator executable
        string generator2DPath = @"./Generator2DApp/bin/Debug/net8.0/Generator2DApp";
        string generator3DPath = @"./Generator3DApp/bin/Debug/net8.0/Generator3DApp";
        string generator4DPath = @"./Generator4DApp/bin/Debug/net8.0/Generator4DApp";

        Run2DSimulations(generator2DPath, 1);
        Run3DSimulations(generator3DPath, 1);
        Run4DSimulations(generator4DPath, 1);
    }

    private void Run2DSimulations(string generator2DPath, int nSimulations)
    {
        // Initialize random with fixed seed for reproducibility
        Random random = new Random(42);

        int[] radiusValues = { 5, 9, 14, 20 };
        double[] radiusMultipliers = { 2.5, 1.25, 0.75 };
        int simulationCount = 0;

        // Systematically iterate through radius values and multipliers
        int radiusIndex = 0;
        int multiplierIndex = 0;

        while (simulationCount < nSimulations)
        {
            // Get current radius and multiplier
            int radius = radiusValues[radiusIndex];
            double radiusMultiplier = radiusMultipliers[multiplierIndex];
            int startingAreaSize = (int)(radius * radiusMultiplier);

            // Randomly modify parameters with 50/25/25 distribution
            double kernelSigmaMultiplier = random.NextDouble() < 0.5 ? 0.175 : (random.NextDouble() < 0.5 ? 0.175 * 1.25 : 0.175 * 0.75);
            double growthSigmaMultiplier = random.NextDouble() < 0.5 ? 0.004 : (random.NextDouble() < 0.5 ? 0.004 * 1.25 : 0.004 * 0.75);
            double cellSpawnChance = 0.2 + (random.NextDouble() * 0.25); // Random value between 0.2 and 0.45
            double growthSteepness = random.NextDouble() < 0.5 ? 4.0 : (random.NextDouble() < 0.5 ? 4.0 * 1.25 : 4.0 * 0.75);
            double center = random.NextDouble() < 0.5 ? 0.16 : (random.NextDouble() < 0.5 ? 0.16 * 1.25 : 0.16 * 0.75);

            string simName = $"2D_R{radius}_RM{radiusMultiplier:F2}_SAS{startingAreaSize}_" +
                           $"KSM{kernelSigmaMultiplier:F4}_GSM{growthSigmaMultiplier:F4}_" +
                           $"CSC{cellSpawnChance:F2}_GS{growthSteepness:F2}_C{center:F2}";

            simulations.Add(new Simulation
            {
                GeneratorPath = generator2DPath,
                Arguments = "--numFrames 50 " +
                           $"--kernelRadius {radius} " +
                           $"--kernelSigmaMultiplier {kernelSigmaMultiplier:F4} " + 
                           $"--growthSigmaMultiplier {growthSigmaMultiplier:F4} " +
                           $"--center {center:F2} " +
                           "--deltaT 0.1 " +
                           $"--startingAreaSize {startingAreaSize} " +
                           $"--cellSpawnChance {cellSpawnChance:F2} " +
                           "--minInitialValue 0.2 " +
                           "--maxInitialValue 1.0 " +
                           $"--growthSteepness {growthSteepness:F2} " +
                           $"--outputDirectory {Path.Combine(OUTPUTS_DIR, TWO_D_DIR, simName)} " +
                           "--maxFrameTimeSeconds 2.0"
            });

            // Increment indices
            multiplierIndex++;
            if (multiplierIndex >= radiusMultipliers.Length)
            {
                multiplierIndex = 0;
                radiusIndex++;
                if (radiusIndex >= radiusValues.Length)
                {
                    radiusIndex = 0;
                }
            }

            simulationCount++;
        }
    }

    private void Run3DSimulations(string generator3DPath, int nSimulations)
    {
        Random random = new Random(43);  // Different seed from 2D

        int[] radiusValues = { 2, 4, 5, 7 };  
        double[] radiusMultipliers = { 2.0, 1.25, 0.75 };
        int simulationCount = 0;

        int radiusIndex = 0;
        int multiplierIndex = 0;

        while (simulationCount < nSimulations)
        {
            int radius = radiusValues[radiusIndex];
            double radiusMultiplier = radiusMultipliers[multiplierIndex];
            int startingAreaSize = (int)(radius * radiusMultiplier);

            double kernelSigmaMultiplier = random.NextDouble() < 0.5 ? 0.175 : (random.NextDouble() < 0.5 ? 0.175 * 1.25 : 0.175 * 0.75);
            double growthSigmaMultiplier = random.NextDouble() < 0.5 ? 0.004 : (random.NextDouble() < 0.5 ? 0.004 * 1.25 : 0.004 * 0.75);
            double cellSpawnChance = 0.3 + (random.NextDouble() * 0.3); // Random value between 0.3 and 0.6
            double growthSteepness = random.NextDouble() < 0.5 ? 4.0 : (random.NextDouble() < 0.5 ? 4.0 * 1.25 : 4.0 * 0.75);
            double center = random.NextDouble() < 0.5 ? 0.16 : (random.NextDouble() < 0.5 ? 0.16 * 1.25 : 0.16 * 0.75);

            string simName = $"3D_R{radius}_RM{radiusMultiplier:F2}_SAS{startingAreaSize}_" +
                           $"KSM{kernelSigmaMultiplier:F4}_GSM{growthSigmaMultiplier:F4}_" +
                           $"CSC{cellSpawnChance:F2}_GS{growthSteepness:F2}_C{center:F2}";

            simulations.Add(new Simulation
            {
                GeneratorPath = generator3DPath,
                Arguments = "--numFrames 50 " +
                           $"--kernelRadius {radius} " +
                           $"--kernelSigmaMultiplier {kernelSigmaMultiplier:F4} " +
                           $"--growthSigmaMultiplier {growthSigmaMultiplier:F4} " +
                           $"--center {center:F2} " +
                           "--deltaT 0.1 " +
                           $"--startingAreaSize {startingAreaSize} " +
                           $"--cellSpawnChance {cellSpawnChance:F2} " +
                           "--minInitialValue 0.2 " +
                           "--maxInitialValue 1.0 " +
                           $"--growthSteepness {growthSteepness:F2} " +
                           $"--outputDirectory {Path.Combine(OUTPUTS_DIR, THREE_D_DIR, simName)} " +
                           "--maxFrameTimeSeconds 2.5"
            });

            multiplierIndex++;
            if (multiplierIndex >= radiusMultipliers.Length)
            {
                multiplierIndex = 0;
                radiusIndex++;
                if (radiusIndex >= radiusValues.Length)
                {
                    radiusIndex = 0;
                }
            }

            simulationCount++;
        }
    }

    private void Run4DSimulations(string generator4DPath, int nSimulations)
    {
        Random random = new Random(44);  // Different seed from 2D and 3D

        int[] radiusValues = { 2, 3, 4, 5 };  // Smaller than 3D due to tesseract growth
        double[] radiusMultipliers = { 1.75, 1.25, 0.75 };
        int simulationCount = 0;

        int radiusIndex = 0;
        int multiplierIndex = 0;

        while (simulationCount < nSimulations)
        {
            int radius = radiusValues[radiusIndex];
            double radiusMultiplier = radiusMultipliers[multiplierIndex];
            int startingAreaSize = (int)(radius * radiusMultiplier);

            double kernelSigmaMultiplier = random.NextDouble() < 0.5 ? 0.125 : (random.NextDouble() < 0.5 ? 0.125 * 1.25 : 0.125 * 0.75);
            double growthSigmaMultiplier = random.NextDouble() < 0.5 ? 0.012 : (random.NextDouble() < 0.5 ? 0.012 * 1.25 : 0.012 * 0.75);
            double cellSpawnChance = 0.4 + (random.NextDouble() * 0.4); // Random value between 0.4 and 0.8
            double growthSteepness = random.NextDouble() < 0.5 ? 4.0 : (random.NextDouble() < 0.5 ? 4.0 * 1.25 : 4.0 * 0.75);
            double center = random.NextDouble() < 0.5 ? 0.15 : (random.NextDouble() < 0.5 ? 0.15 * 1.25 : 0.15 * 0.75);

            string simName = $"4D_R{radius}_RM{radiusMultiplier:F2}_SAS{startingAreaSize}_" +
                           $"KSM{kernelSigmaMultiplier:F4}_GSM{growthSigmaMultiplier:F4}_" +
                           $"CSC{cellSpawnChance:F2}_GS{growthSteepness:F2}_C{center:F2}";

            simulations.Add(new Simulation
            {
                GeneratorPath = generator4DPath,
                Arguments = "--numFrames 50 " +
                           $"--kernelRadius {radius} " +
                           $"--kernelSigmaMultiplier {kernelSigmaMultiplier:F4} " +
                           $"--growthSigmaMultiplier {growthSigmaMultiplier:F4} " +
                           $"--center {center:F2} " +
                           "--deltaT 0.1 " +
                           $"--startingAreaSize {startingAreaSize} " +
                           $"--cellSpawnChance {cellSpawnChance:F2} " +
                           "--minInitialValue 0.2 " +
                           "--maxInitialValue 1.0 " +
                           $"--growthSteepness {growthSteepness:F2} " +
                           $"--outputDirectory {Path.Combine(OUTPUTS_DIR, FOUR_D_DIR, simName)} " +
                           "--maxFrameTimeSeconds 3.0"
            });

            multiplierIndex++;
            if (multiplierIndex >= radiusMultipliers.Length)
            {
                multiplierIndex = 0;
                radiusIndex++;
                if (radiusIndex >= radiusValues.Length)
                {
                    radiusIndex = 0;
                }
            }

            simulationCount++;
        }
    }

    public async Task RunSimulations()
    {
        Console.WriteLine("Starting simulations...");

        foreach (var simulation in simulations)
        {
            await RunGenerator(simulation.GeneratorPath, simulation.Arguments);
            Console.WriteLine();
        }

        Console.WriteLine("All simulations completed.");
    }

    private async Task RunGenerator(string executablePath, string arguments)
    {
        Console.WriteLine($"Starting generator: {executablePath} with arguments: {arguments}");

        using Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        // Capture and log output and errors
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine(e.Data);
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"Error: {e.Data}");
        };

        // Start process
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Wait for the process to complete
        await process.WaitForExitAsync();

        Console.WriteLine($"Generator finished: {executablePath}");
    }

    public static async Task Main(string[] args)
    {
        GeneratorManager manager = new GeneratorManager();
        await manager.RunSimulations();
    }

    public static void MoveToCoolSimulations(string simulationPath)
    {
        // Determine the dimension from the path
        string dimension;
        if (simulationPath.Contains("/2D/") || simulationPath.Contains("\\2D\\"))
            dimension = TWO_D_DIR;
        else if (simulationPath.Contains("/3D/") || simulationPath.Contains("\\3D\\"))
            dimension = THREE_D_DIR;
        else if (simulationPath.Contains("/4D/") || simulationPath.Contains("\\4D\\"))
            dimension = FOUR_D_DIR;
        else
            throw new ArgumentException("Cannot determine dimension from path");

        // Get the simulation name
        string simulationName = Path.GetFileName(simulationPath);
        
        // Construct the destination path
        string destinationPath = Path.Combine(COOL_SIMULATIONS_DIR, dimension, simulationName);
        
        // If the destination already exists, delete it
        if (Directory.Exists(destinationPath))
            Directory.Delete(destinationPath, true);
        
        // Move the simulation to cool_simulations
        Directory.Move(simulationPath, destinationPath);
        
        Console.WriteLine($"Moved simulation to cool_simulations: {destinationPath}");
    }
}
