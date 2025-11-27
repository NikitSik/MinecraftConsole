using System.Numerics;

namespace MinecraftConsole;

/// <summary>
/// Отвечает за гравитацию, ускорение и разрешение коллизий игрока с блоками.
/// </summary>
public static class Physics
{
    /// <summary>Ускорение свободного падения.</summary>
    private const float Gravity = -18f;
    /// <summary>Скорость, с которой горизонтальная скорость приближается к желаемой.</summary>
    private const float Acceleration = 18f;
    /// <summary>Затухание скорости в воздухе.</summary>
    private const float AirFriction = 0.9f;
    /// <summary>Затухание скорости на земле.</summary>
    private const float GroundFriction = 0.7f;

    /// <summary>
    /// Выполняет один физический шаг: применяет силы и корректирует положение.
    /// </summary>
    public static void Step(Player player, InputHandler input, World world, float deltaTime)
    {
        // Горизонтальное ускорение на основе ввода
        if (input.DesiredMove != Vector3.Zero)
        {
            float speed = input.Sprint ? 10f : 6f;
            Vector3 desired = input.DesiredMove * speed;
            Vector3 currentHorizontal = new(player.Velocity.X, 0f, player.Velocity.Z);
            currentHorizontal = Vector3.Lerp(currentHorizontal, desired, Acceleration * deltaTime / speed);
            player.Velocity = new Vector3(currentHorizontal.X, player.Velocity.Y, currentHorizontal.Z);
        }
        else
        {
            player.Velocity *= player.OnGround ? GroundFriction : AirFriction;
        }

        // Гравитация
        player.Velocity += new Vector3(0f, Gravity * deltaTime, 0f);

        player.OnGround = false;
        Vector3 position = player.Position;

        // Перемещение по каждой оси с отдельной проверкой столкновений
        Vector3 attempt = position + new Vector3(player.Velocity.X * deltaTime, 0f, 0f);
        position = ResolveAxis(player, position, attempt, world, axis: 0);

        attempt = position + new Vector3(0f, 0f, player.Velocity.Z * deltaTime);
        position = ResolveAxis(player, position, attempt, world, axis: 2);

        attempt = position + new Vector3(0f, player.Velocity.Y * deltaTime, 0f);
        position = ResolveAxis(player, position, attempt, world, axis: 1);

        player.Position = position;
    }

    /// <summary>
    /// Разрешает коллизию при движении по одной оси, возвращая скорректированную позицию.
    /// </summary>
    private static Vector3 ResolveAxis(Player player, Vector3 originalPos, Vector3 desiredPos, World world, int axis)
    {
        Vector3 minDesired = new(desiredPos.X - Player.Radius, desiredPos.Y, desiredPos.Z - Player.Radius);
        Vector3 maxDesired = new(desiredPos.X + Player.Radius, desiredPos.Y + Player.Height, desiredPos.Z + Player.Radius);

        int startX = (int)Math.Floor(minDesired.X);
        int endX = (int)Math.Ceiling(maxDesired.X);
        int startY = (int)Math.Floor(minDesired.Y);
        int endY = (int)Math.Ceiling(maxDesired.Y);
        int startZ = (int)Math.Floor(minDesired.Z);
        int endZ = (int)Math.Ceiling(maxDesired.Z);

        Vector3 resolved = desiredPos;

        for (int x = startX; x <= endX; x++)
        for (int y = startY; y <= endY; y++)
        for (int z = startZ; z <= endZ; z++)
        {
            if (!world.InBounds(x, y, z)) continue;
            BlockType type = world.GetBlock(x, y, z);
            if (type == BlockType.Air || type == BlockType.Water) continue;

            if (Intersects(desiredPos, x, y, z))
            {
                switch (axis)
                {
                    case 0:
                        resolved.X = originalPos.X;
                        player.Velocity = new Vector3(0f, player.Velocity.Y, player.Velocity.Z);
                        break;
                    case 1:
                        if (desiredPos.Y < originalPos.Y)
                        {
                            resolved.Y = y + 1f;
                            player.OnGround = true;
                        }
                        else
                        {
                            resolved.Y = y - Player.Height;
                        }
                        player.Velocity = new Vector3(player.Velocity.X, 0f, player.Velocity.Z);
                        break;
                    case 2:
                        resolved.Z = originalPos.Z;
                        player.Velocity = new Vector3(player.Velocity.X, player.Velocity.Y, 0f);
                        break;
                }
            }
        }

        return resolved;
    }

    /// <summary>
    /// Проверяет, пересекает ли капсула игрока блок по координатам блока.
    /// </summary>
    private static bool Intersects(Vector3 desiredPos, int bx, int by, int bz)
    {
        float minX = desiredPos.X - Player.Radius;
        float maxX = desiredPos.X + Player.Radius;
        float minY = desiredPos.Y;
        float maxY = desiredPos.Y + Player.Height;
        float minZ = desiredPos.Z - Player.Radius;
        float maxZ = desiredPos.Z + Player.Radius;

        return maxX > bx && minX < bx + 1 && maxY > by && minY < by + 1 && maxZ > bz && minZ < bz + 1;
    }
}
