using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class GeneratorManager
{
    public async Task RunSimulations()
    {
        // Define paths to each generator executable
        string generator2DPath = @"./Generator2DApp/bin/Debug/net8.0/Generator2DApp";
        string generator3DPath = @"./Generator3DApp/bin/Debug/net8.0/Generator3DApp";
        string generator4DPath = @"./Generator4DApp/bin/Debug/net8.0/Generator4DApp";

        // Define full command-line arguments for each simulation with all parameters
        string simulation1Args2D = "--numFrames 1 --kernelRadius 5 --kernelSigmaMultiplier 0.125 --growthSigmaMultiplier 0.125 " +
                                   "--center 0.15 --deltaT 1.0 --startingAreaSize 30 --cellSpawnChance 0.1 --minInitialValue 0.1 " +
                                   "--maxInitialValue 1.0 --outputDirectory Output2D_Simulation1";
        
        string simulation1Args3D = "--numFrames 5 --kernelRadius 3 --kernelSigmaMultiplier 0.125 --growthSigmaMultiplier 0.125 " +
                                   "--center 0.15 --deltaT 1.0 --startingAreaSize 8 --cellSpawnChance 0.2 --minInitialValue 0.2 " +
                                   "--maxInitialValue 1.0 --outputDirectory Output3D_Simulation1";

        string simulation1Args4D = "--numFrames 1 --kernelRadius 2 --kernelSigmaMultiplier 0.125 --growthSigmaMultiplier 0.1 " +
                                   "--center 0.15 --deltaT 0.1 --startingAreaSize 3 --cellSpawnChance 0.25 --minInitialValue 0.1 " +
                                   "--maxInitialValue 1.0 --outputDirectory Output4D_Simulation1";

        string simulation2Args2D = "--numFrames 1 --kernelRadius 6 --kernelSigmaMultiplier 0.1 --growthSigmaMultiplier 0.15 " +
                                   "--center 0.2 --deltaT 0.5 --startingAreaSize 20 --cellSpawnChance 0.15 --minInitialValue 0.2 " +
                                   "--maxInitialValue 0.8 --outputDirectory Output2D_Simulation2";

        string simulation2Args3D = "--numFrames 1 --kernelRadius 3 --kernelSigmaMultiplier 0.125 --growthSigmaMultiplier 0.125 " +
                                   "--center 0.18 --deltaT 1.0 --startingAreaSize 6 --cellSpawnChance 0.3 --minInitialValue 0.1 " +
                                   "--maxInitialValue 1.0 --outputDirectory Output3D_Simulation2";

        string simulation2Args4D = "--numFrames 1 --kernelRadius 2 --kernelSigmaMultiplier 0.15 --growthSigmaMultiplier 0.12 " +
                                   "--center 0.2 --deltaT 0.2 --startingAreaSize 3 --cellSpawnChance 0.3 --minInitialValue 0.2 " +
                                   "--maxInitialValue 1.0 --outputDirectory Output4D_Simulation2";

        // Run first set of simulations
        Console.WriteLine("Starting Simulation 1...");
        await RunGenerator(generator2DPath, simulation1Args2D);
        await RunGenerator(generator3DPath, simulation1Args3D);
        await RunGenerator(generator4DPath, simulation1Args4D);

        Console.WriteLine("Simulation 1 completed.\n");

        // Run second set of simulations
        Console.WriteLine("Starting Simulation 2...");
        await RunGenerator(generator2DPath, simulation2Args2D);
        await RunGenerator(generator3DPath, simulation2Args3D);
        await RunGenerator(generator4DPath, simulation2Args4D);

        Console.WriteLine("Simulation 2 completed.\n");

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
