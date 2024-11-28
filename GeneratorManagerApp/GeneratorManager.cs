using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
                    $"--maxFrameTimeSeconds {5:F1}"
            });
        }

        /*
        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 4 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.012 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 5 " +
                        "--cellSpawnChance 0.4 " +
                        "--minInitialValue 0.2 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius4_KSM0125_GSM012_StartArea4 " +
                        "--maxFrameTimeSeconds 12"
        });


        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 15 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius5_GSM0035" + 
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 10 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.00385 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 23 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius10_GSM00385 " +
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 12 " +
                        "--kernelSigmaMultiplier 0.13 " +
                        "--growthSigmaMultiplier 0.0032 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 18 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius12_KSM013_GSM0032 " +
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 24 " +
                        "--kernelSigmaMultiplier 0.15 " +
                        "--growthSigmaMultiplier 0.004 " +
                        "--center 0.16 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 18 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius24_KSM015_GSM004 " +
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 3 " +
                        "--kernelSigmaMultiplier 0.12 " +
                        "--growthSigmaMultiplier 0.0033 " +
                        "--center 0.14 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 2 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius3_KSM012_GSM0033 " +
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 16 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.00375 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 48 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius16_GSM00375 " +
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 8 " +
                        "--kernelSigmaMultiplier 0.11 " +
                        "--growthSigmaMultiplier 0.0039 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 18 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius8_KSM011_GSM0039 " +
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 2 " +
                        "--kernelSigmaMultiplier 0.13 " +
                        "--growthSigmaMultiplier 0.0032 " +
                        "--center 0.14 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 3 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius2_KSM013_GSM0032 " +
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 10 " +
                        "--kernelSigmaMultiplier 0.127 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 8 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius10_KSM0127_GSM0035 " +
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 6 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.00345 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 4 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius6_GSM00345 " +
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 12 " +
                        "--kernelSigmaMultiplier 0.12 " +
                        "--growthSigmaMultiplier 0.0036 " +
                        "--center 0.14 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 36 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius12_KSM012_GSM0036 " +
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator2DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 4 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0034 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 9 " +
                        "--cellSpawnChance 0.75 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output2D_Radius4_GSM0034 " +
                        "--maxFrameTimeSeconds 2"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 4 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea4_0_500Frames " +
                        "--maxFrameTimeSeconds 3"
        });



        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 5 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea5_0_500Frames " +
                        "--maxFrameTimeSeconds 3"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 6 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea5_5_500Frames " +
                        "--maxFrameTimeSeconds 3"
        });


        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 4 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea4_0_500Frames_2 " +
                        "--maxFrameTimeSeconds 3"
        });



        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 5 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea5_0_500Frames_2 " +
                        "--maxFrameTimeSeconds 3"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 6 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea5_5_500Frames_2 " +
                        "--maxFrameTimeSeconds 3"
        });



        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 4 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea4_0_500Frames_3 " +
                        "--maxFrameTimeSeconds 3"
        });



        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 5.0 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea5_0_500Frames_3 " +
                        "--maxFrameTimeSeconds 3"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 6 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea5_5_500Frames_3 " +
                        "--maxFrameTimeSeconds 3"
        });



        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 4 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea4_0_500Frames_4 " +
                        "--maxFrameTimeSeconds 3"
        });



        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 5 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea5_0_500Frames_4 " +
                        "--maxFrameTimeSeconds 3"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator3DPath,
            Arguments = "--numFrames 500 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0035 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 6 " +
                        "--cellSpawnChance 0.7 " +
                        "--minInitialValue 0.5 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output3D_Radius5_StartArea5_5_500Frames_4 " +
                        "--maxFrameTimeSeconds 3"
        });


        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 3 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.012 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 2 " +
                        "--cellSpawnChance 0.4 " +
                        "--minInitialValue 0.2 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius3_GSM012 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 4 " +
                        "--kernelSigmaMultiplier 0.13 " +
                        "--growthSigmaMultiplier 0.0115 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 12 " +
                        "--cellSpawnChance 0.5 " +
                        "--minInitialValue 0.3 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius4_KSM013_GSM0115 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.12 " +
                        "--growthSigmaMultiplier 0.0125 " +
                        "--center 0.16 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 11 " +
                        "--cellSpawnChance 0.35 " +
                        "--minInitialValue 0.25 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius5_KSM012_GSM0125 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 6 " +
                        "--kernelSigmaMultiplier 0.127 " +
                        "--growthSigmaMultiplier 0.011 " +
                        "--center 0.14 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 9 " +
                        "--cellSpawnChance 0.45 " +
                        "--minInitialValue 0.35 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius6_KSM0127_GSM011 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 8 " +
                        "--kernelSigmaMultiplier 0.115 " +
                        "--growthSigmaMultiplier 0.013 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 6 " +
                        "--cellSpawnChance 0.4 " +
                        "--minInitialValue 0.2 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius8_KSM0115_GSM013 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 2 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.0105 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 2 " +
                        "--cellSpawnChance 0.4 " +
                        "--minInitialValue 0.3 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius2_GSM0105 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 10 " +
                        "--kernelSigmaMultiplier 0.135 " +
                        "--growthSigmaMultiplier 0.014 " +
                        "--center 0.16 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 30 " +
                        "--cellSpawnChance 0.35 " +
                        "--minInitialValue 0.25 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius10_KSM0135_GSM014 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 7 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.011 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 16 " +
                        "--cellSpawnChance 0.45 " +
                        "--minInitialValue 0.4 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius7_GSM011 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 3 " +
                        "--kernelSigmaMultiplier 0.122 " +
                        "--growthSigmaMultiplier 0.012 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 5 " +
                        "--cellSpawnChance 0.5 " +
                        "--minInitialValue 0.3 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius3_KSM0122_GSM012 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 9 " +
                        "--kernelSigmaMultiplier 0.130 " +
                        "--growthSigmaMultiplier 0.0108 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 7 " +
                        "--cellSpawnChance 0.4 " +
                        "--minInitialValue 0.25 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius9_KSM013_GSM0108 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 6 " +
                        "--kernelSigmaMultiplier 0.115 " +
                        "--growthSigmaMultiplier 0.0122 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 4 " +
                        "--cellSpawnChance 0.35 " +
                        "--minInitialValue 0.3 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius6_KSM0115_GSM0122 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 5 " +
                        "--kernelSigmaMultiplier 0.125 " +
                        "--growthSigmaMultiplier 0.012 " +
                        "--center 0.14 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 15 " +
                        "--cellSpawnChance 0.45 " +
                        "--minInitialValue 0.3 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius5_GSM012 " +
                        "--maxFrameTimeSeconds 6"
        });

        simulations.Add(new Simulation
        {
            GeneratorPath = generator4DPath,
            Arguments = "--numFrames 50 " +
                        "--kernelRadius 4 " +
                        "--kernelSigmaMultiplier 0.127 " +
                        "--growthSigmaMultiplier 0.0125 " +
                        "--center 0.15 " +
                        "--deltaT 0.1 " +
                        "--startingAreaSize 12 " +
                        "--cellSpawnChance 0.35 " +
                        "--minInitialValue 0.2 " +
                        "--maxInitialValue 1.0 " +
                        "--outputDirectory Output4D_Radius4_GSM0125 " +
                        "--maxFrameTimeSeconds 6"
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
}
