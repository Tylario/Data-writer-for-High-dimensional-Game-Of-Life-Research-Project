using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

public class GeneratorManager
{
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
        // Initialize simulations
        InitializeSimulations();
    }

    private void InitializeSimulations()
    {
        // Define paths to each generator executable
        string generator2DPath = @"./Generator2DApp/bin/Debug/net8.0/Generator2DApp";
        string generator3DPath = @"./Generator3DApp/bin/Debug/net8.0/Generator3DApp";
        string generator4DPath = @"./Generator4DApp/bin/Debug/net8.0/Generator4DApp";

        // Working Simulation Examples

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 100 " +
                        "--kernelRadius 8 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 16 " +
                        "--cellSpawnChance 0.4 " +
                        "--minInitialValue 0.2 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Simulations/test2D " +
                        "--maxFrameTimeSeconds 3"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 300 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 4 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Simulations/test3D " +
                        "--maxFrameTimeSeconds 3"
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
                        "--outputDirectory Simulations/test4D " +
                        "--maxFrameTimeSeconds 5"
        });


        /*

        // Batch simulations

        // 2D simulations

        int[] kernelRadii2D = { 2, 3, 5, 7, 10 };
        double[] kernelSigmaMultipliers2D = { 0.04, 0.1, 0.125, 0.15, 0.25 };
        double[] growthSigmaMultipliers2D = { 0.0015, 0.003, 0.0035, 0.0045, 0.01 };
        double[] centers2D = { 0.13, 0.15, 0.17 };
        int[] startingAreaSizes2D = { 3, 4, 5, 6, 8, 10, 12 };
        double[] cellSpawnChances2D = { 0.2, 0.4, 0.55 };
        double[] growthSteepnesses2D = { 2.0, 4.0, 7.0 };

        foreach (int kr in kernelRadii2D)
        foreach (double ksm in kernelSigmaMultipliers2D)
        foreach (double gsm in growthSigmaMultipliers2D)
        foreach (double center in centers2D)
        foreach (int sas in startingAreaSizes2D)
        foreach (double csc in cellSpawnChances2D)
        foreach (double gs in growthSteepnesses2D)
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
                    $"--outputDirectory Simulations/2D_KR{kr}_KSM{ksm:F4}_GSM{gsm:F4}_C{center:F2}_SAS{sas}_CSC{csc:F2}_GS{gs:F1} " +
                    $"--maxFrameTimeSeconds {0.5:F1} " +
                    $"--growthSteepness {gs:F1}"
            });
        }


        // 3D


        int[] kernelRadii3D = { 5, 6, 7, 8, 10 };
        double[] kernelSigmaMultipliers3D = { 0.125, 0.15, 0.175, 0.2, 0.25 };
        double[] growthSigmaMultipliers3D = { 0.0035, 0.004, 0.0045, 0.005, 0.01 };
        double[] centers3D = { 0.15, 0.16, 0.17 };
        int[] startingAreaSizes3D = { 4, 5, 6, 8, 10, 12 };
        double[] cellSpawnChances3D = { 0.2, 0.4, 0.55 };
        double[] growthSteepnesses3D = { 2.0, 4.0, 7.0 };

        foreach (int kr in kernelRadii3D)
        foreach (double ksm in kernelSigmaMultipliers3D)
        foreach (double gsm in growthSigmaMultipliers3D)
        foreach (double center in centers3D)
        foreach (int sas in startingAreaSizes3D)
        foreach (double csc in cellSpawnChances3D)
        foreach (double gs in growthSteepnesses3D)
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
                    $"--outputDirectory Simulations/3D_KR{kr}_KSM{ksm:F4}_GSM{gsm:F4}_C{center:F2}_SAS{sas}_CSC{csc:F2}_GS{gs:F1} " +
                    $"--maxFrameTimeSeconds {1.5:F1}"
            });
        }


        // 4D simulations

        int[] kernelRadii4D = { 2, 3, 4, 5 };
        double[] kernelSigmaMultipliers4D = { 0.125, 0.15, 0.175, 0.2, 0.25 };
        double[] growthSigmaMultipliers4D = { 0.012, 0.015, 0.0175, 0.02, 0.025 };
        double[] centers4D = { 0.15, 0.16, 0.17 };
        int[] startingAreaSizes4D = { 3, 5, 6, 7, 9 };
        double[] cellSpawnChances4D = { 0.2, 0.4, 0.55 };
        double[] growthSteepnesses4D = { 2.0, 4.0, 7.0 };

        foreach (int kr in kernelRadii4D)
        foreach (double ksm in kernelSigmaMultipliers4D)
        foreach (double gsm in growthSigmaMultipliers4D)
        foreach (double center in centers4D)
        foreach (int sas in startingAreaSizes4D)
        foreach (double csc in cellSpawnChances4D)
        foreach (double gs in growthSteepnesses4D)
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
                    $"--cellSpawnChance {csc:F2} " +
                    "--minInitialValue 0.2 " +
                    "--maxInitialValue 1.0 " +
                    $"--growthSteepness {gs:F1} " +
                    $"--outputDirectory Simulations/4D_KR{kr}_KSM{ksm:F4}_GSM{gsm:F4}_C{center:F2}_SAS{sas}_CSC{csc:F2}_GS{gs:F1} " +
                    $"--maxFrameTimeSeconds {5:F1}"
            });
        }

        */
        
    }
    

    public async Task RunSimulations()
    {
        // Create Simulations directory if it doesn't exist
        string simulationsPath = Path.Combine(Directory.GetCurrentDirectory(), "Simulations");
        
        // Check available disk space
        DriveInfo drive = new DriveInfo(Path.GetPathRoot(simulationsPath));
        long minimumRequiredSpace = 1024L * 1024L * 1024L; // 1GB in bytes
        
        if (drive.AvailableFreeSpace < minimumRequiredSpace)
        {
            Console.WriteLine($"ERROR: Insufficient disk space. Available: {drive.AvailableFreeSpace / (1024*1024)}MB");
            Console.WriteLine("Simulations cancelled - require at least 1GB free space");
            return;
        }

        Directory.CreateDirectory(simulationsPath);

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
