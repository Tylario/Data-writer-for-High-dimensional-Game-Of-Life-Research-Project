using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

public class GeneratorManager
{
    private const string OUTPUTS_DIR = "outputs";
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
            Path.Combine(OUTPUTS_DIR, FOUR_D_DIR)
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

            //run multiple simulations with randomly generated parameters
            
            int nSimulations = 1;

            Run2DSimulations(generator2DPath, nSimulations);
            Run3DSimulations(generator3DPath, nSimulations);
            Run4DSimulations(generator4DPath, nSimulations);

        /*
        //Or add a single simulation here:
        
            simulations.Add(new Simulation
            {
                GeneratorPath = generator2DPath,
                Arguments = "--numFrames 75 " +
                           "--kernelRadius 9 " +
                           "--kernelSigmaMultiplier 0.175 " +
                           "--growthSigmaMultiplier 0.004 " +
                           "--center 0.16 " +
                           "--deltaT 0.1 " +
                           "--startingAreaSize 12 " +
                           "--cellSpawnChance 0.3 " +
                           "--minInitialValue 0.2 " +
                           "--maxInitialValue 1.0 " +
                           "--growthSteepness 4.0 " +
                           "--outputDirectory 2D_Example " +
                           "--maxFrameTimeSeconds 2.0"
            });

            simulations.Add(new Simulation
            {
                GeneratorPath = generator3DPath,
                Arguments = "--numFrames 100 " +
                           "--kernelRadius 6 " +
                           "--kernelSigmaMultiplier 0.175 " +
                           "--growthSigmaMultiplier 0.004 " +
                           "--center 0.16 " +
                           "--deltaT 0.1 " +
                           "--startingAreaSize 8 " +
                           "--cellSpawnChance 0.4 " +
                           "--minInitialValue 0.2 " +
                           "--maxInitialValue 1.0 " +
                           "--growthSteepness 4.0 " +
                           "--outputDirectory 3D_Example " +
                           "--maxFrameTimeSeconds 1.5"
            });

            simulations.Add(new Simulation
            {
                GeneratorPath = generator4DPath,
                Arguments = "--numFrames 75 " +
                           "--kernelRadius 4 " +
                           "--kernelSigmaMultiplier 0.125 " +
                           "--growthSigmaMultiplier 0.012 " +
                           "--center 0.15 " +
                           "--deltaT 0.1 " +
                           "--startingAreaSize 6 " +
                           "--cellSpawnChance 0.5 " +
                           "--minInitialValue 0.2 " +
                           "--maxInitialValue 1.0 " +
                           "--growthSteepness 4.0 " +
                           "--outputDirectory 4D_Example " +
                           "--maxFrameTimeSeconds 5.0"
            });
        */
    }

    private void Run2DSimulations(string generator2DPath, int nSimulations)
    {
        Random random = new Random(42);

        // Kernel radius - controls size of neighborhood that affects each cell
        int[] radiusValues = { 3, 4, 5, 6, 7, 9, 10, 12 };
        
        // Kernel sigma multiplier - controls spread of kernel function relative to radius
        double[] kernelSigmaMultipliers = { 0.15, 0.175, 0.2, 0.225 };
        
        // Growth sigma multiplier - controls spread of growth function relative to radius
        double[] growthSigmaMultipliers = { 0.003, 0.004, 0.005, 0.006 };
        
        // Probability of spawning a cell in the initial state
        double[] cellSpawnChances = { 0.35, 0.4, 0.45 };
        
        // Controls steepness of growth function curve
        double[] growthSteepnessValues = { 3.5, 4.0, 4.5, 5.0 };
        
        // Center of growth function - optimal neighborhood sum for growth
        double[] centerValues = { 0.14, 0.16, 0.18, 0.2 };
        
        // Number of initial clusters of cells
        int[] startingPointsValues = { 1, 1, 2, 3, 3, 4 };
        
        // Controls how far apart initial clusters can be placed
        int[] offsetMultipliers = { 6, 8, 10, 12,};
        
        // Multiplier for starting area size relative to radius
        double[] radiusMultipliers = { 0.9, 1.0, 1.1, 1.2 };
        
        // Maximum total cell mass allowed before simulation ends
        float[] maxCellMassMultipliers = {4.0f};
        
        // Range of possible initial cell values
        double[] minInitialValues = { 0.1, 0.15, 0.2, 0.25 };
        double[] maxInitialValues = { 0.8, 0.9, 1.0 };
        
        // Time step size for simulation
        double[] deltaTValues = { 0.1 };

        for (int i = 0; i < nSimulations; i++)
        {
            // Randomly select values from each set
            int radius = radiusValues[random.Next(radiusValues.Length)];
            double radiusMultiplier = radiusMultipliers[random.Next(radiusMultipliers.Length)];
            int startingAreaSize = (int)(radius * radiusMultiplier);
            
            double kernelSigmaMultiplier = kernelSigmaMultipliers[random.Next(kernelSigmaMultipliers.Length)];
            double growthSigmaMultiplier = growthSigmaMultipliers[random.Next(growthSigmaMultipliers.Length)];
            double cellSpawnChance = cellSpawnChances[random.Next(cellSpawnChances.Length)];
            double growthSteepness = growthSteepnessValues[random.Next(growthSteepnessValues.Length)];
            double center = centerValues[random.Next(centerValues.Length)];
            int startingPoints = startingPointsValues[random.Next(startingPointsValues.Length)];
            
            int offsetMultiplier = offsetMultipliers[random.Next(offsetMultipliers.Length)];
            int randomOffsetRange = radius * offsetMultiplier;
            float maxCellMassMultiplier = maxCellMassMultipliers[random.Next(maxCellMassMultipliers.Length)];
            float maxCellMass = (float)Math.Pow(radius, 2) * maxCellMassMultiplier;
            
            double minInitialValue = minInitialValues[random.Next(minInitialValues.Length)];
            double maxInitialValue = maxInitialValues[random.Next(maxInitialValues.Length)];
            double deltaT = deltaTValues[random.Next(deltaTValues.Length)];

            string simName = $"2D_R{radius}_RM{radiusMultiplier:F2}_SAS{startingAreaSize}_" +
                           $"KSM{kernelSigmaMultiplier:F4}_GSM{growthSigmaMultiplier:F4}_" +
                           $"CSC{cellSpawnChance:F2}_GS{growthSteepness:F2}_C{center:F2}_" +
                           $"SP{startingPoints}_ROR{randomOffsetRange}";

            simulations.Add(new Simulation
            {
                GeneratorPath = generator2DPath,
                Arguments = "--numFrames 500 " +
                           $"--kernelRadius {radius} " +
                           $"--kernelSigmaMultiplier {kernelSigmaMultiplier:F4} " + 
                           $"--growthSigmaMultiplier {growthSigmaMultiplier:F4} " +
                           $"--center {center:F2} " +
                           $"--deltaT {deltaT:F1} " +
                           $"--startingAreaSize {startingAreaSize} " +
                           $"--cellSpawnChance {cellSpawnChance:F2} " +
                           $"--minInitialValue {minInitialValue:F2} " +
                           $"--maxInitialValue {maxInitialValue:F2} " +
                           $"--growthSteepness {growthSteepness:F2} " +
                           $"--startingPoints {startingPoints} " +
                           $"--randomOffsetRange {randomOffsetRange} " +
                           $"--outputDirectory {Path.Combine(OUTPUTS_DIR, TWO_D_DIR, simName)} " +
                           "--maxFrameTimeSeconds 1.0 " +
                           $"--maxCellMass {maxCellMass:F1}"
            });
        }
    }

    private void Run3DSimulations(string generator3DPath, int nSimulations)
    {
        Random random = new Random(43);

        // Kernel radius - controls size of neighborhood that affects each cell
        int[] radiusValues = { 2, 3, 4, 5, 6, 7 };
        
        // Kernel sigma multiplier - controls spread of kernel function relative to radius
        double[] kernelSigmaMultipliers = { 0.15, 0.175, 0.2, 0.225 };
        
        // Growth sigma multiplier - controls spread of growth function relative to radius
        double[] growthSigmaMultipliers = { 0.003, 0.004, 0.005, 0.006 };
        
        // Probability of spawning a cell in the initial state
        double[] cellSpawnChances = { 0.3, 0.35, 0.4, 0.45, 0.5, 0.6 };
        
        // Controls steepness of growth function curve
        double[] growthSteepnessValues = { 3.5, 4.0, 4.5, 5.0 };
        
        // Center of growth function - optimal neighborhood sum for growth
        double[] centerValues = { 0.14, 0.16, 0.18, 0.2 };
        
        // Number of initial clusters of cells
        int[] startingPointsValues = { 2, 3, 4 };
        
        // Controls how far apart initial clusters can be placed
        int[] offsetMultipliers = { 1, 2, 3, 4, 5 };
        
        // Multiplier for starting area size relative to radius
        double[] radiusMultipliers = { 0.9, 1.0, 1.1, 1.2 };
        
        // Maximum total cell mass allowed before simulation ends
        float[] maxCellMassMultipliers = { 1.0f };
        
        // Range of possible initial cell values
        double[] minInitialValues = { 0.1, 0.2, 0.3, 0.4 };
        double[] maxInitialValues = { 0.6, 0.7, 0.8, 0.9 };
        
        // Time step size for simulation
        double[] deltaTValues = { 0.1 };

        for (int i = 0; i < nSimulations; i++)
        {
            // Randomly select values from each set
            int radius = radiusValues[random.Next(radiusValues.Length)];
            double radiusMultiplier = radiusMultipliers[random.Next(radiusMultipliers.Length)];
            int startingAreaSize = (int)(radius * radiusMultiplier);
            
            double kernelSigmaMultiplier = kernelSigmaMultipliers[random.Next(kernelSigmaMultipliers.Length)];
            double growthSigmaMultiplier = growthSigmaMultipliers[random.Next(growthSigmaMultipliers.Length)];
            double cellSpawnChance = cellSpawnChances[random.Next(cellSpawnChances.Length)];
            double growthSteepness = growthSteepnessValues[random.Next(growthSteepnessValues.Length)];
            double center = centerValues[random.Next(centerValues.Length)];
            int startingPoints = startingPointsValues[random.Next(startingPointsValues.Length)];
            
            int offsetMultiplier = offsetMultipliers[random.Next(offsetMultipliers.Length)];
            int randomOffsetRange = radius * offsetMultiplier;
            float maxCellMassMultiplier = maxCellMassMultipliers[random.Next(maxCellMassMultipliers.Length)];
            float maxCellMass = (float)Math.Pow(radius, 4) * maxCellMassMultiplier;

            double minInitialValue = minInitialValues[random.Next(minInitialValues.Length)];
            double maxInitialValue = maxInitialValues[random.Next(maxInitialValues.Length)];
            double deltaT = deltaTValues[random.Next(deltaTValues.Length)];

            string simName = $"3D_R{radius}_RM{radiusMultiplier:F2}_SAS{startingAreaSize}_" +
                           $"KSM{kernelSigmaMultiplier:F4}_GSM{growthSigmaMultiplier:F4}_" +
                           $"CSC{cellSpawnChance:F2}_GS{growthSteepness:F2}_C{center:F2}_" +
                           $"SP{startingPoints}_ROR{randomOffsetRange}";

            simulations.Add(new Simulation
            {
                GeneratorPath = generator3DPath,
                Arguments = "--numFrames 250 " +
                           $"--kernelRadius {radius} " +
                           $"--kernelSigmaMultiplier {kernelSigmaMultiplier:F4} " +
                           $"--growthSigmaMultiplier {growthSigmaMultiplier:F4} " +
                           $"--center {center:F2} " +
                           $"--deltaT {deltaT:F1} " +
                           $"--startingAreaSize {startingAreaSize} " +
                           $"--cellSpawnChance {cellSpawnChance:F2} " +
                           $"--minInitialValue {minInitialValue:F2} " +
                           $"--maxInitialValue {maxInitialValue:F2} " +
                           $"--growthSteepness {growthSteepness:F2} " +
                           $"--startingPoints {startingPoints} " +
                           $"--randomOffsetRange {randomOffsetRange} " +
                           $"--outputDirectory {Path.Combine(OUTPUTS_DIR, THREE_D_DIR, simName)} " +
                           "--maxFrameTimeSeconds 0.5 " +
                           $"--maxCellMass {maxCellMass:F1}"
            });
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
}
