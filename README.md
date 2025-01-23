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
    Arguments = "--numFrames 75 " +                // Total number of simulation frames to generate
               "--kernelRadius 9 " +               // Size of neighborhood that affects each cell
               "--kernelSigmaMultiplier 0.175 " +  // Controls spread of kernel function relative to radius
               "--growthSigmaMultiplier 0.004 " +  // Controls spread of growth function relative to radius
               "--center 0.16 " +                  // Target density for stable growth (optimal neighborhood sum)
               "--deltaT 0.1 " +                   // Time step size for simulation updates
               "--startingAreaSize 12 " +          // Size of initial area where cells can spawn
               "--cellSpawnChance 0.3 " +          // Probability of spawning a cell in the initial state
               "--minInitialValue 0.2 " +          // Minimum possible initial cell value
               "--maxInitialValue 1.0 " +          // Maximum possible initial cell value
               "--growthSteepness 4.0 " +          // Controls sharpness of growth function response
               "--startingPoints 1 " +             // Number of initial clusters of cells
               "--randomOffsetRange 90 " +         // Maximum distance between initial cell clusters
               "--maxCellMass 324.0 " +           // Maximum total cell mass before simulation ends
               "--outputDirectory 2D_Example " +    // Directory where simulation data will be saved
               "--maxFrameTimeSeconds 2.0"         // Maximum time allowed per frame calculation
});

// 3D Example
simulations.Add(new Simulation
{
    GeneratorPath = generator3DPath,
    Arguments = "--numFrames 100 " +               // Total number of simulation frames to generate
               "--kernelRadius 6 " +               // Size of neighborhood that affects each cell
               "--kernelSigmaMultiplier 0.175 " +  // Controls spread of kernel function relative to radius
               "--growthSigmaMultiplier 0.004 " +  // Controls spread of growth function relative to radius
               "--center 0.16 " +                  // Target density for stable growth (optimal neighborhood sum)
               "--deltaT 0.1 " +                   // Time step size for simulation updates
               "--startingAreaSize 8 " +           // Size of initial area where cells can spawn
               "--cellSpawnChance 0.4 " +          // Probability of spawning a cell in the initial state
               "--minInitialValue 0.2 " +          // Minimum possible initial cell value
               "--maxInitialValue 1.0 " +          // Maximum possible initial cell value
               "--growthSteepness 4.0 " +          // Controls sharpness of growth function response
               "--startingPoints 2 " +             // Number of initial clusters of cells
               "--randomOffsetRange 18 " +         // Maximum distance between initial cell clusters
               "--maxCellMass 1296.0 " +          // Maximum total cell mass before simulation ends
               "--outputDirectory 3D_Example " +    // Directory where simulation data will be saved
               "--maxFrameTimeSeconds 1.5"         // Maximum time allowed per frame calculation
});

// 4D Example
simulations.Add(new Simulation
{
    GeneratorPath = generator4DPath,
    Arguments = "--numFrames 75 " +                // Total number of simulation frames to generate
               "--kernelRadius 4 " +               // Size of neighborhood that affects each cell
               "--kernelSigmaMultiplier 0.125 " +  // Controls spread of kernel function relative to radius
               "--growthSigmaMultiplier 0.012 " +  // Controls spread of growth function relative to radius
               "--center 0.15 " +                  // Target density for stable growth (optimal neighborhood sum)
               "--deltaT 0.1 " +                   // Time step size for simulation updates
               "--startingAreaSize 6 " +           // Size of initial area where cells can spawn
               "--cellSpawnChance 0.5 " +          // Probability of spawning a cell in the initial state
               "--minInitialValue 0.2 " +          // Minimum possible initial cell value
               "--maxInitialValue 1.0 " +          // Maximum possible initial cell value
               "--growthSteepness 4.0 " +          // Controls sharpness of growth function response
               "--startingPoints 3 " +             // Number of initial clusters of cells
               "--randomOffsetRange 12 " +         // Maximum distance between initial cell clusters
               "--maxCellMass 256.0 " +           // Maximum total cell mass before simulation ends
               "--outputDirectory 4D_Example " +    // Directory where simulation data will be saved
               "--maxFrameTimeSeconds 5.0"         // Maximum time allowed per frame calculation
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
- Open `viewer.html` in your browser to view and interact with the simulations. Click the "Upload Folder" button and select a simulation folder containing frame .json files.

#### Controls:
- **Playback Controls:** Play/Pause, Restart, Previous Frame, Next Frame
- **2D Simulations:** Use scroll wheel to zoom in and out
- **3D Simulations:** 
  - Mouse for camera rotation
  - Toggle auto-rotation
  - Cross section view with +/- controls and auto-scan option
- **4D Simulations:**
  - Same controls as 3D
  - W dimension controls (+/-) with auto-scan option
  - Cross section controls for both 3D and W dimensions

## Results

### Patterns Observed

#### [2D Simulation (View at 0:00)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=0)
#### [3D Simulation (View at 5:05)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=305)
#### [4D Simulation (View at 9:29)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=569)

- **Unstable Patterns:** Patterns that change sporadically and unpredictably, exhibiting fluctuating growth and shrinkage rates. These often evolve into one of the other defined patterns. [(View at 1:54)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=114)
  
  [![Unstable Patterns](https://www.tylar.io/assets/images/simulationImages/unstable.PNG)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=114)

- **Explosive Growth:** Rapid and continuous expansion of live cells, usually at the same speed in all directions. The pattern always appears to be growing, with no immediate signs of decay. It spreads evenly and maintains its growth without significant fluctuation. This often results in a chaotic spread, where cells grow uncontrollably, sometimes creating a cellular or dotted appearance with evenly distributed mini-stable points. Some of these points may remain stable, while others morph or shift slowly. [(View at 0:20)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=20)
  
  [![Explosive Growth](https://www.tylar.io/assets/images/simulationImages/explosiveGrowth.PNG)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=20)

- **Decay:** Patterns that gradually die out, either due to insufficient support from neighbors or overcrowding. [(View at 0:05)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=5)
  
  [![Decay](https://www.tylar.io/assets/images/simulationImages/decay.PNG)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=5)

- **Static Equilibrium:** Patterns that stabilize into fixed shapes, usually as circles/sphere, which do not change over time. [(View at 3:59)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=239)
  
  [![Static Equilibrium](https://www.tylar.io/assets/images/simulationImages/staticEquilibrium.PNG)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=239)

- **Oscillating Patterns:** Patterns that pulse or oscillate between states, appearing to expand and contract while maintaining symmetry. [(View at 4:26)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=266)
  
  [![Oscillating Patterns](https://www.tylar.io/assets/images/simulationImages/oscillatingPattern.PNG)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=266)

- **Gliders:** Moving patterns that traverse the grid in a consistent direction, resembling structures like gliders in Conway's 2D Game of Life. [(View at 4:07)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=247)
  
  [![Gliders](https://www.tylar.io/assets/images/simulationImages/gliders.PNG)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=247)

- **Rotating Structures:** Patterns that appear to spin or rotate around a central axis, often forming intricate loops and spirals. [(View at 2:47)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=167)
  
  [![Rotating Structures](https://www.tylar.io/assets/images/simulationImages/rotatingStructure.PNG)](https://www.tylar.io/assets/videos/SimulationResults.mp4#t=167)

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

1. **State Transition Equation:**
   ```
   A(t+1) = clamp[0,1]( A(t) + Δt * (2G(K*A(t)) - 1) )
   ```
   Where A(t) is the cell state at time t, Δt is the time step, G is the growth function, K is the kernel function, and * denotes convolution.

2. **Kernel Function:**
   ```
   K(r) = exp(-(r - r₀)² / (2σₖ²))
   ```
   - Gaussian function centered at r₀ = kernelRadius/2
   - σₖ = kernelRadius * kernelSigmaMultiplier
   - Circular in 2D, spherical in 3D, and hyperspherical in 4D

3. **Growth Function:**
   ```
   G(x) = exp(-s(x - c)² / (2σᵧ²))
   ```
   - x is the convolution value
   - c is the center (target density)
   - s is the growth steepness
   - σᵧ = kernelRadius * growthSigmaMultiplier

4. **Key Parameters:**
   - `kernelRadius`: Size of neighborhood influence
   - `kernelSigmaMultiplier`: Controls kernel spread
   - `growthSigmaMultiplier`: Controls growth sensitivity
   - `growthSteepness`: Controls sharpness of growth response
   - `center`: Target density for stable growth
   - `deltaT`: Time step size for state updates

