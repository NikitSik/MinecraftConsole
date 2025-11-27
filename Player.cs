using System.Numerics;

namespace MinecraftConsole;

/// <summary>
/// Представляет игрока: его координаты, скорость и ориентацию в пространстве.
/// </summary>
public class Player
{
    /// <summary>Текущая позиция игрока в мировых координатах.</summary>
    public Vector3 Position { get; set; } = new(32f, 20f, 32f);
    /// <summary>Скорость игрока по осям X, Y и Z.</summary>
    public Vector3 Velocity { get; set; } = Vector3.Zero;
    /// <summary>Поворот вокруг вертикальной оси (в радианах).</summary>
    public float Yaw { get; set; } = 0f;
    /// <summary>Поворот вверх/вниз (в радианах).</summary>
    public float Pitch { get; set; } = 0f;
    /// <summary>Флаг, находится ли игрок на земле (нужен для прыжка и трения).</summary>
    public bool OnGround { get; set; }

    /// <summary>Рост персонажа в метрах условных единиц.</summary>
    public const float Height = 1.8f;
    /// <summary>Радиус коллизии вокруг игрока.</summary>
    public const float Radius = 0.3f;
}
