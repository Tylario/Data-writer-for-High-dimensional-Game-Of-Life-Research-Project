using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

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

        // 2D  
        /*

        int[] kernelRadii = { 2, 3, 5, 7, 10 };
        double[] kernelSigmaMultipliers = { 0.04, 0.1, 0.125, 0.15, 0.25 };
        double[] growthSigmaMultipliers = { 0.0015, 0.003, 0.0035, 0.0045, 0.01 };
        double[] centers = { 0.13, 0.15, 0.17 };
        int[] startingAreaSizes = { 3, 4, 5, 6, 8, 10, 12 };
        double[] cellSpawnChances = { 0.2, 0.4, 0.55 };
        double[] growthSteepnesses = { 2.0, 4.0, 7.0 };

        foreach (int kr in kernelRadii)
        foreach (double ksm in kernelSigmaMultipliers)
        foreach (double gsm in growthSigmaMultipliers)
        foreach (double center in centers)
        foreach (int sas in startingAreaSizes)
        foreach (double csc in cellSpawnChances)
        foreach (double gs in growthSteepnesses)
        {
            simulations.Add(new Simulation
            {
                GeneratorPath = generator2DPath,
                Arguments = 
                    "--numFrames 800 " +
                    $"--kernelRadius {kr} " +
                    $"--kernelSigmaMultiplier {ksm:F4} " +
                    $"--growthSigmaMultiplier {gsm:F4} " +
                    $"--center {center:F2} " +
                    "--deltaT 0.1 " +
                    $"--startingAreaSize {sas} " +
                    $"--cellSpawnChance {csc:F2} " +
                    "--minInitialValue 0.2 " +
                    "--maxInitialValue 1.0 " +
                    $"--outputDirectory 2D_KR{kr}_KSM{ksm:F4}_GSM{gsm:F4}_C{center:F2}_SAS{sas}_CSC{csc:F2}_GS{gs:F1} " +
                    $"--maxFrameTimeSeconds {0.4:F1} " +
                    $"--growthSteepness {gs:F1}"
            });
        }

        */

        // 3D

        /*

        int[] kernelRadii = { 5, 6, 7, 8, 10 };
        double[] kernelSigmaMultipliers = { 0.125, 0.15, 0.175, 0.2, 0.25 };
        double[] growthSigmaMultipliers = { 0.0035, 0.004, 0.0045, 0.005, 0.01 };
        double[] centers = { 0.15, 0.16, 0.17 };
        int[] startingAreaSizes = { 4, 5, 6, 8, 10, 12 };
        double[] cellSpawnChances = { 0.2, 0.4, 0.55 };
        double[] growthSteepnesses = { 2.0, 4.0, 7.0 };

        foreach (int kr in kernelRadii)
        foreach (double ksm in kernelSigmaMultipliers)
        foreach (double gsm in growthSigmaMultipliers)
        foreach (double center in centers)
        foreach (int sas in startingAreaSizes)
        foreach (double csc in cellSpawnChances)
        foreach (double gs in growthSteepnesses)
        {
            simulations.Add(new Simulation
            {
                GeneratorPath = generator3DPath,
                Arguments = "--numFrames 500 " +
                    $"--kernelRadius {kr} " +
                    $"--kernelSigmaMultiplier {ksm:F4} " +
                    $"--growthSigmaMultiplier {gsm:F4} " +
                    $"--center {center:F2} " +
                    "--deltaT 0.1 " +
                    $"--startingAreaSize {sas} " +
                    $"--cellSpawnChance {csc:F2} " +
                    "--minInitialValue 0.2 " +
                    "--maxInitialValue 1.0 " +
                    $"--growthSteepness {gs:F1} " +
                    $"--outputDirectory 3D_KR{kr}_KSM{ksm:F4}_GSM{gsm:F4}_C{center:F2}_SAS{sas}_CSC{csc:F2}_GS{gs:F1} " +
                    $"--maxFrameTimeSeconds {1.5:F1}"
            });
        }

        */

        // 4D

        /*

        int[] kernelRadii = { 2, 3, 4, 6, 8 };
        double[] kernelSigmaMultipliers = { 0.125, 0.15, 0.175, 0.2, 0.25 };
        double[] growthSigmaMultipliers = { 0.012, 0.015, 0.0175, 0.02, 0.025 };
        double[] centers = { 0.15, 0.16, 0.17 };
        int[] startingAreaSizes = { 3, 5, 6, 7, 9 };

        foreach (int kr in kernelRadii)
        foreach (double ksm in kernelSigmaMultipliers)
        foreach (double gsm in growthSigmaMultipliers)
        foreach (double center in centers)
        foreach (int sas in startingAreaSizes)
        {
            simulations.Add(new Simulation
            {
                GeneratorPath = generator4DPath,
                Arguments = "--numFrames 500 " +
                    $"--kernelRadius {kr} " +
                    $"--kernelSigmaMultiplier {ksm:F4} " +
                    $"--growthSigmaMultiplier {gsm:F4} " +
                    $"--center {center:F2} " +
                    "--deltaT 0.1 " +
                    $"--startingAreaSize {sas} " +
                    $"--cellSpawnChance 0.35 " +
                    "--minInitialValue 0.25 " +
                    "--maxInitialValue 1.0 " +
                    $"--outputDirectory 4D_KR{kr}_KSM{ksm:F4}_GSM{gsm:F4}_C{center:F2}_SAS{sas} " +
                    $"--maxFrameTimeSeconds {3:F1}"
            });
        }
    
        */

        for (int i = 0; i < 1; i++)
        {
            simulations.Add(new Simulation
            {
                GeneratorPath = generator2DPath,
                Arguments = "--numFrames 50 " +
                            "--kernelRadius 20 " +
                            "--kernelSigmaMultiplier 0.175 " + 
                            "--growthSigmaMultiplier 0.004 " +
                            "--center 0.16 " +
                            "--deltaT 0.1 " +
                            "--startingAreaSize 90 " +
                            "--cellSpawnChance 0.3 " +
                            "--minInitialValue 0.2 " +
                            "--maxInitialValue 1.0 " +
                            "--growthSteepness 4.0 " +
                            $"--outputDirectory {Path.Combine(OUTPUTS_DIR, TWO_D_DIR, $"2D_Example_{i}")} " +
                            "--maxFrameTimeSeconds 2.0"
            });
        }


        /*
        // 3D Example

        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
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
            Arguments = "--numFrames 10 " +
                        "--kernelRadius 3 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.012 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 5 " +
                        "--cellSpawnChance 0.6 " +
                        "--minInitialValue 0.2 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius3_GSM012 " +
                        "--maxFrameTimeSeconds 5"
        });

        */
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
