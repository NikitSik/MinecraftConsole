namespace MinecraftConsole;

/// <summary>
/// Хранит блоки мира и предоставляет методы для чтения и записи.
/// </summary>
public class World
{
    /// <summary>Количество чанков по оси X.</summary>
    public const int ChunkCountX = 8;
    /// <summary>Количество чанков по оси Y.</summary>
    public const int ChunkCountY = 4;
    /// <summary>Количество чанков по оси Z.</summary>
    public const int ChunkCountZ = 8;
    /// <summary>Полная ширина мира в блоках.</summary>
    public const int Width = ChunkCountX * Chunk.Size;
    /// <summary>Полная высота мира в блоках.</summary>
    public const int Height = ChunkCountY * Chunk.Size;
    /// <summary>Полная глубина мира в блоках.</summary>
    public const int Depth = ChunkCountZ * Chunk.Size;

    /// <summary>Трёхмерный массив чанков, формирующий мир.</summary>
    private readonly Chunk[,,] _chunks = new Chunk[ChunkCountX, ChunkCountY, ChunkCountZ];

    /// <summary>
    /// Создаёт мир, заполняя каждую ячейку новым пустым чанком.
    /// </summary>
    public World()
    {
        for (int x = 0; x < ChunkCountX; x++)
        for (int y = 0; y < ChunkCountY; y++)
        for (int z = 0; z < ChunkCountZ; z++)
            _chunks[x, y, z] = new Chunk();
    }

    /// <summary>Проверяет, находится ли координата внутри мира.</summary>
    public bool InBounds(int x, int y, int z) =>
        x >= 0 && y >= 0 && z >= 0 && x < Width && y < Height && z < Depth;

    /// <summary>Возвращает тип блока по координатам или Air, если координата вне мира.</summary>
    public BlockType GetBlock(int x, int y, int z)
    {
        if (!InBounds(x, y, z)) return BlockType.Air;
        (int cx, int cy, int cz, int lx, int ly, int lz) = Translate(x, y, z);
        return _chunks[cx, cy, cz].GetBlock(lx, ly, lz);
    }

    /// <summary>Устанавливает тип блока, если координата в пределах мира.</summary>
    public void SetBlock(int x, int y, int z, BlockType type)
    {
        if (!InBounds(x, y, z)) return;
        (int cx, int cy, int cz, int lx, int ly, int lz) = Translate(x, y, z);
        _chunks[cx, cy, cz].SetBlock(lx, ly, lz, type);
    }

    /// <summary>Перебирает все блоки мира в виде перечисления.</summary>
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

    /// <summary>Преобразует мировые координаты в координаты чанка и локальные координаты внутри него.</summary>
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
