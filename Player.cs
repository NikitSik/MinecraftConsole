using System.Numerics;

namespace MinecraftConsole;

/// <summary>
/// Represents the player avatar, including position, rotation and velocity.
/// </summary>
public class Player
{
    public Vector3 Position { get; set; } = new(32f, 20f, 32f);
    public Vector3 Velocity { get; set; } = Vector3.Zero;
    public float Yaw { get; set; } = 0f;   // Left/right rotation in radians
    public float Pitch { get; set; } = 0f; // Up/down rotation in radians
    public bool OnGround { get; set; }

    public const float Height = 1.8f;
    public const float Radius = 0.3f;
}
