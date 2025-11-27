namespace MinecraftConsole;

/// <summary>
/// Serializes the world to a compact binary format so play sessions can resume later.
/// </summary>
public class SaveSystem
{
    public void SaveWorld(World world, string path)
    {
        using FileStream stream = new(path, FileMode.Create, FileAccess.Write);
        using BinaryWriter writer = new(stream);
        writer.Write(World.Width);
        writer.Write(World.Height);
        writer.Write(World.Depth);
        foreach ((int x, int y, int z, BlockType type) in world.EnumerateAllBlocks())
        {
            writer.Write((byte)type);
        }
    }

    public World? LoadWorld(string path)
    {
        if (!File.Exists(path)) return null;
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
        using BinaryReader reader = new(stream);
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        int depth = reader.ReadInt32();
        if (width != World.Width || height != World.Height || depth != World.Depth) return null;

        World world = new();
        for (int x = 0; x < World.Width; x++)
        for (int y = 0; y < World.Height; y++)
        for (int z = 0; z < World.Depth; z++)
        {
            BlockType type = (BlockType)reader.ReadByte();
            world.SetBlock(x, y, z, type);
        }

        return world;
    }
}
