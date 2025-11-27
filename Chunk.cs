namespace MinecraftConsole;

/// <summary>
/// Represents a small sub-section of the world to reduce indexing costs.
/// </summary>
public class Chunk
{
    public const int Size = 8;
    private readonly BlockType[,,] _blocks = new BlockType[Size, Size, Size];

    public BlockType GetBlock(int x, int y, int z) => _blocks[x, y, z];

    public void SetBlock(int x, int y, int z, BlockType type) => _blocks[x, y, z] = type;
}
