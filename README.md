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
You can customize the simulations by modifying the `GeneratorManager.cs` file in the repository. In the `InitializeSimulations()` method, you can either add custom simulations or run multiple random simulations.

#### Adding Custom Simulations:
```csharp
// 2D Example
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

// 3D Example
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

// 4D Example
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
```

#### Running Random Simulations:
```csharp
// Set number of simulations to run
int nSimulations = 1;

// Run simulations for each dimension
Run2DSimulations(generator2DPath, nSimulations);
Run3DSimulations(generator3DPath, nSimulations);
Run4DSimulations(generator4DPath, nSimulations);
```

#### Random Simulation Parameter Ranges:
When using the Run[2/3/4]DSimulations methods, parameters are randomly generated within these ranges:

- **2D Simulations:**
  - Kernel Radius: 5-20
  - Cell Spawn Chance: 0.2-0.45
  - Starting Area Size: Based on radius × multiplier (0.75-2.5)

- **3D Simulations:**
  - Kernel Radius: 2-7
  - Cell Spawn Chance: 0.3-0.6
  - Starting Area Size: Based on radius × multiplier (0.75-2.0)

- **4D Simulations:**
  - Kernel Radius: 3-6
  - Cell Spawn Chance: 0.4-0.8
  - Starting Area Size: Based on radius × multiplier (0.75-1.75)

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

