namespace MinecraftConsole;

/// <summary>
/// Небольшая часть мира, содержащая фиксированное количество блоков.
/// </summary>
public class Chunk
{
    /// <summary>Размер чанка по каждой оси в блоках.</summary>
    public const int Size = 8;
    /// <summary>Блоки, расположенные внутри данного чанка.</summary>
    private readonly BlockType[,,] _blocks = new BlockType[Size, Size, Size];

    /// <summary>Возвращает тип блока по локальным координатам чанка.</summary>
    public BlockType GetBlock(int x, int y, int z) => _blocks[x, y, z];

    /// <summary>Устанавливает тип блока по локальным координатам чанка.</summary>
    public void SetBlock(int x, int y, int z, BlockType type) => _blocks[x, y, z] = type;
}
