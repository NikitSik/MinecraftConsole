# MinecraftConsole

Simple console-based voxel engine inspired by Minecraft. Requires only the .NET SDK (net10.0).

## Prerequisites
- .NET 10 SDK installed (verify with `dotnet --version`, expected `10.0.100` or newer preview).
- A terminal that supports ANSI cursor positioning (Windows Terminal, macOS/Linux terminals, or PowerShell 7+).

## Running
From the repository root:

```bash
dotnet restore  # optional, no external packages are used
dotnet build   # compile to ensure everything is ready
dotnet run     # launch the game
```

If `dotnet` is unavailable in your environment, install the .NET 10 SDK from https://dotnet.microsoft.com/download/dotnet/10.0.

## Controls
- **W/A/S/D**: Move forward/left/back/right
- **Arrow Left/Right**: Turn camera
- **Arrow Up/Down**: Look up/down
- **Space**: Jump
- **Shift**: Sprint
- **E**: Break the targeted block
- **Q**: Place a Dirt block just before the targeted block
- **Esc**: Quit (you will be prompted to save)

## Saves
- Save file is written to `world.save` in the working directory.
- On launch, the game loads `world.save` if present; otherwise it generates a new world.

## Troubleshooting
- If the console flickers, try reducing the console size or lowering the render constants in `RayCaster.cs` (screen size, FOV, and render distance).
- Ensure the working directory remains the project root so the save file is found.
