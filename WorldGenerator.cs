namespace MinecraftConsole;

/// <summary>
/// Builds a simple layered height-map based world using deterministic noise.
/// </summary>
public class WorldGenerator
{
    private readonly Random _random;

    public WorldGenerator(int seed)
    {
        _random = new Random(seed);
    }

    public void Populate(World world)
    {
        for (int x = 0; x < World.Width; x++)
        for (int z = 0; z < World.Depth; z++)
        {
            int height = ComputeHeight(x, z);
            for (int y = 0; y < World.Height; y++)
            {
                if (y > height) world.SetBlock(x, y, z, BlockType.Air);
                else if (y == height) world.SetBlock(x, y, z, BlockType.Grass);
                else if (y >= height - 2) world.SetBlock(x, y, z, BlockType.Dirt);
                else world.SetBlock(x, y, z, BlockType.Stone);
            }
        }
    }

    private int ComputeHeight(int x, int z)
    {
        double nx = x / 12.0;
        double nz = z / 12.0;
        double hill = Math.Sin(nx) + Math.Cos(nz);
        double detail = Math.Sin(nx * 0.5) * Math.Cos(nz * 0.5);
        double noise = hill * 2 + detail + _random.NextDouble() * 0.3;
        int baseHeight = World.Height / 3;
        int height = baseHeight + (int)(noise * 3);
        return Math.Clamp(height, 1, World.Height - 2);
    }
}
