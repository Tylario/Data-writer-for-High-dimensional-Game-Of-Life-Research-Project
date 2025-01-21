# High-Dimensional Game of Life Simulation

## Instructions

### Build and Run

Run the following commands in your terminal:

**To Build:**
```bash
dotnet clean
dotnet build
```

**To Run:**
```bash
dotnet run --project GeneratorManagerApp
```

### Customizing Simulations
You can customize the simulations by modifying the `GeneratorManager.cs` file in the repository. Specifically, edit the `InitializeSimulations()` method within the `GeneratorManager` class. 

**Example Code Snippet:**
```csharp
private void InitializeSimulations()
{
    // Define paths to each generator executable
    string generator2DPath = @"./Generator2DApp/bin/Debug/net8.0/Generator2DApp";
    string generator3DPath = @"./Generator3DApp/bin/Debug/net8.0/Generator3DApp";
    string generator4DPath = @"./Generator4DApp/bin/Debug/net8.0/Generator4DApp";

    // Example for 3D Simulation
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
}
```

You can uncomment or add different configurations for 2D, 3D, and 4D simulations to explore a wide range of parameters.

### Output and Visualization
- Simulations will output data into folders in the `output` directory.
- Open `viewer.html` in your browser to view and interact with the simulations. Select all the frames you want to include in the render.

#### Controls:
- **2D Simulations:** Use scroll wheel to zoom in and out.
- **3D Simulations:** Use `WASD` for movement, `QE` for vertical adjustments, and mouse for rotation.
- **4D Simulations:** Same controls as 3D, with additional options to toggle the `W` axis view.

---

## Introduction

The **Game of Life**, developed by John Horton Conway, is a two-dimensional cellular automaton that demonstrates how complex patterns and behaviors can emerge from simple rules applied to a grid of cells. My project expands upon this foundational principle by:

1. Exploring higher-dimensional spaces (3D and 4D).
2. Incorporating continuous states and smooth transitions, inspired by SmoothLife and Lenia.
3. Utilizing convolution operations with circular and spherical kernels.

---

## Mathematical Framework

### Conway's Game of Life (2D)
Conway's Game of Life operates on a 2D grid where each cell can be either **alive (1)** or **dead (0)**. The rules for state transitions are:
1. **Survival:** A live cell remains alive if it has 2 or 3 live neighbors.
2. **Birth:** A dead cell becomes alive if it has exactly 3 live neighbors.
3. **Death:** A live cell with fewer than 2 or more than 3 live neighbors dies.

### Extending to 3D and 4D
For higher dimensions, similar rules are generalized:
- **3D:** Each cell checks its 26 neighbors (including diagonals) in a cubic space.
- **4D:** Each cell checks its 80 neighbors in a hypercubic space.

Survival and birth thresholds are adjusted based on the increased number of neighbors. For example:
- **3D:** Survive with 4–5 neighbors, birth with exactly 5.
- **4D:** Survive with 7–8 neighbors, birth with exactly 9.

### Continuous Cellular Automata and Kernels
Instead of binary states (0 or 1), cells can take continuous values between 0 and 1. The next state of a cell depends on a weighted sum of its neighbors, computed using convolution kernels:
1. **Kernel Function:** 
   - A Gaussian function centered at the cell, decreasing in influence outward.
   - Circular in 2D, spherical in 3D, and hyperspherical in 4D.

2. **Convolution Calculation:**
   - Computes a weighted sum of neighboring cell values based on kernel weights.
   - Normalizes the sum to ensure balance.

3. **Growth Function:**
   - Controls cell behavior using parameters like center value and growth steepness.
   - Cells grow if their convolution value is near a specified target range.

4. **Time-Stepping:**
   - Updates states continuously using a time step (`deltaT`) to simulate smooth transitions.

---

## Results

### Patterns Observed
- **Unstable Patterns:** Patterns that change sporadically and unpredictably, exhibiting fluctuating growth and shrinkage rates. These often evolve into one of the other defined patterns
- **Explosive Growth:** Rapid and continuous expansion of live cells, usually at the same speed in all directions. The pattern always appears to be growing, with no immediate signs of decay. It spreads evenly and maintains its growth without significant fluctuation. This often results in a chaotic spread, where cells grow uncontrollably, sometimes creating a cellular or dotted appearance with evenly distributed mini-stable points. Some of these points may remain stable, while others morph or shift slowly
- **Decay:** Patterns that gradually die out, either due to insufficient support from neighbors or overcrowding.
- **Static Equilibrium:** Patterns that stabilize into fixed shapes, usually as circles/sphere, which do not change over time.
- **Oscillating Patterns:** Patterns that pulse or oscillate between states, appearing to expand and contract while maintaining symmetry.
- **Gliders:** Moving patterns that traverse the grid in a consistent direction, resembling structures like gliders in Conway's 2D Game of Life.
- **Rotating Structures:** Patterns that appear to spin or rotate around a central axis, often forming intricate loops and spirals.

