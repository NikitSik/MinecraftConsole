using System.Numerics;

namespace MinecraftConsole;

/// <summary>
/// Считывает нажатия клавиш и формирует команды для движения и взаимодействия.
/// </summary>
public class InputHandler
{
    /// <summary>Флаг запроса выхода из игры.</summary>
    public bool ExitRequested { get; private set; }
    /// <summary>Флаг запроса разрушения блока перед игроком.</summary>
    public bool BreakRequested { get; private set; }
    /// <summary>Флаг запроса установки нового блока перед игроком.</summary>
    public bool PlaceRequested { get; private set; }
    /// <summary>Нормализованный вектор желаемого движения по плоскости XZ.</summary>
    public Vector3 DesiredMove { get; private set; }
    /// <summary>Активирован ли бег (Shift).</summary>
    public bool Sprint { get; private set; }

    /// <summary>
    /// Обрабатывает все нажатия, накопившиеся в буфере консоли, и обновляет состояние игрока.
    /// </summary>
    public void ProcessInput(Player player)
    {
        ExitRequested = false;
        BreakRequested = false;
        PlaceRequested = false;
        DesiredMove = Vector3.Zero;
        Sprint = false;

        int moveForward = 0;
        int moveRight = 0;

        while (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Modifiers.HasFlag(ConsoleModifiers.Shift))
            {
                Sprint = true;
            }

            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    ExitRequested = true;
                    break;
                case ConsoleKey.E:
                    BreakRequested = true;
                    break;
                case ConsoleKey.Q:
                    PlaceRequested = true;
                    break;
                case ConsoleKey.W:
                    moveForward++;
                    break;
                case ConsoleKey.S:
                    moveForward--;
                    break;
                case ConsoleKey.A:
                    moveRight--;
                    break;
                case ConsoleKey.D:
                    moveRight++;
                    break;
                case ConsoleKey.LeftArrow:
                    player.Yaw -= 0.08f;
                    break;
                case ConsoleKey.RightArrow:
                    player.Yaw += 0.08f;
                    break;
                case ConsoleKey.UpArrow:
                    player.Pitch = Math.Clamp(player.Pitch + 0.05f, -1.2f, 1.2f);
                    break;
                case ConsoleKey.DownArrow:
                    player.Pitch = Math.Clamp(player.Pitch - 0.05f, -1.2f, 1.2f);
                    break;
                case ConsoleKey.Spacebar:
                    if (player.OnGround)
                    {
                        player.Velocity = new Vector3(player.Velocity.X, 6.5f, player.Velocity.Z);
                        player.OnGround = false;
                    }
                    break;
            }
        }

        if (moveForward != 0 || moveRight != 0)
        {
            Vector3 forward = new((float)Math.Sin(player.Yaw), 0f, (float)Math.Cos(player.Yaw));
            Vector3 right = new(forward.Z, 0f, -forward.X);
            DesiredMove = Vector3.Normalize(forward * moveForward + right * moveRight);
        }
    }
}
