namespace MinecraftConsole;

/// <summary>
/// Holds all chunks and provides helpers for reading/writing block data.
/// </summary>
public class World
{
    public const int ChunkCountX = 8;
    public const int ChunkCountY = 4;
    public const int ChunkCountZ = 8;
    public const int Width = ChunkCountX * Chunk.Size;
    public const int Height = ChunkCountY * Chunk.Size;
    public const int Depth = ChunkCountZ * Chunk.Size;

    private readonly Chunk[,,] _chunks = new Chunk[ChunkCountX, ChunkCountY, ChunkCountZ];

    public World()
    {
        for (int x = 0; x < ChunkCountX; x++)
        for (int y = 0; y < ChunkCountY; y++)
        for (int z = 0; z < ChunkCountZ; z++)
            _chunks[x, y, z] = new Chunk();
    }

    public bool InBounds(int x, int y, int z) =>
        x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Depth;

    public BlockType GetBlock(int x, int y, int z)
    {
        if (!InBounds(x, y, z)) return BlockType.Air;
        (int cx, int cy, int cz, int lx, int ly, int lz) = Translate(x, y, z);
        return _chunks[cx, cy, cz].GetBlock(lx, ly, lz);
    }

    public void SetBlock(int x, int y, int z, BlockType type)
    {
        if (!InBounds(x, y, z)) return;
        (int cx, int cy, int cz, int lx, int ly, int lz) = Translate(x, y, z);
        _chunks[cx, cy, cz].SetBlock(lx, ly, lz, type);
    }

    public IEnumerable<(int x, int y, int z, BlockType type)> EnumerateAllBlocks()
    {
        for (int x = 0; x < Width; x++)
        for (int y = 0; y < Height; y++)
        for (int z = 0; z < Depth; z++)
        {
            BlockType type = GetBlock(x, y, z);
            yield return (x, y, z, type);
        }
    }

    private static (int cx, int cy, int cz, int lx, int ly, int lz) Translate(int x, int y, int z)
    {
        int cx = x / Chunk.Size;
        int cy = y / Chunk.Size;
        int cz = z / Chunk.Size;
        int lx = x % Chunk.Size;
        int ly = y % Chunk.Size;
        int lz = z % Chunk.Size;
        return (cx, cy, cz, lx, ly, lz);
    }
}
