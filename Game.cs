using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace MinecraftConsole;

/// <summary>
/// Управляет жизненным циклом игры: загрузкой, обработкой ввода, обновлением и выводом на экран.
/// </summary>
public class Game
{
    /// <summary>Путь к сохранённому миру на диске.</summary>
    private const string SavePath = "world.save";
    /// <summary>Желаемое время кадра в миллисекундах (~30 FPS).</summary>
    private const int FrameTimeMs = 33;

    /// <summary>Текущий игрок, за которого управляет пользователь.</summary>
    private readonly Player _player = new();
    /// <summary>Компонент, читающий нажатия клавиш и превращающий их в команды.</summary>
    private readonly InputHandler _input = new();
    /// <summary>Простейший консольный рендерер, выполняющий трассировку лучей.</summary>
    private readonly RayCaster _rayCaster = new();
    /// <summary>Ответственен за сохранение и загрузку мира.</summary>
    private readonly SaveSystem _saveSystem = new();
    /// <summary>Игровой мир, содержащий блоки и чанки.</summary>
    private World _world = new();

    /// <summary>
    /// Запускает главный цикл, пока пользователь не попросит выйти.
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;
        LoadOrGenerateWorld();

        Stopwatch frameTimer = Stopwatch.StartNew();
        bool running = true;
        while (running)
        {
            float deltaTime = (float)frameTimer.Elapsed.TotalSeconds;
            frameTimer.Restart();

            _input.ProcessInput(_player);
            if (_input.ExitRequested)
            {
                running = !HandleExit();
            }

            Physics.Step(_player, _input, _world, deltaTime);
            HandleInteraction();

            _rayCaster.Render(_world, _player, out BlockType? targetBlock);
            string hud = BuildHud(targetBlock);
            string frame = _rayCaster.ComposeFrame(hud);
            Console.SetCursorPosition(0, 0);
            Console.Write(frame);

            int sleep = FrameTimeMs - (int)frameTimer.ElapsedMilliseconds;
            if (sleep > 0) Thread.Sleep(sleep);
        }

        Console.CursorVisible = true;
    }

    /// <summary>
    /// Загружает ранее сохранённый мир или создаёт новый, если сохранение отсутствует.
    /// </summary>
    private void LoadOrGenerateWorld()
    {
        World? loaded = _saveSystem.LoadWorld(SavePath);
        if (loaded != null)
        {
            _world = loaded;
        }
        else
        {
            _world = new World();
            WorldGenerator generator = new(Environment.TickCount);
            generator.Populate(_world);
        }
    }

    /// <summary>
    /// Спрашивает пользователя о сохранении перед выходом и прекращает работу цикла.
    /// </summary>
    private bool HandleExit()
    {
        Console.SetCursorPosition(0, RayCaster.ScreenHeight + 1);
        Console.Write("Сохранить мир перед выходом? (Y/N): ");
        ConsoleKey key = Console.ReadKey(true).Key;
        if (key == ConsoleKey.Y)
        {
            _saveSystem.SaveWorld(_world, SavePath);
        }
        return true;
    }

    /// <summary>
    /// Обрабатывает ломание и установку блоков в направлении взгляда игрока.
    /// </summary>
    private void HandleInteraction()
    {
        (Vector3? hitPos, Vector3? placePos) = RayPick(_player.Position, _player.Yaw, _player.Pitch);
        if (_input.BreakRequested && hitPos is Vector3 hp)
        {
            _world.SetBlock((int)hp.X, (int)hp.Y, (int)hp.Z, BlockType.Air);
        }
        if (_input.PlaceRequested && placePos is Vector3 pp)
        {
            _world.SetBlock((int)pp.X, (int)pp.Y, (int)pp.Z, BlockType.Dirt);
        }
    }

    /// <summary>
    /// Ищет блок, в который смотрит игрок, и позицию для установки нового блока рядом.
    /// </summary>
    private (Vector3? hitBlock, Vector3? placePosition) RayPick(Vector3 origin, float yaw, float pitch)
    {
        Vector3 direction = new(
            (float)(Math.Sin(yaw) * Math.Cos(pitch)),
            (float)Math.Sin(pitch),
            (float)(Math.Cos(yaw) * Math.Cos(pitch)));

        Vector3 previous = origin;
        for (float d = 0f; d < RayCaster.MaxRenderDistance; d += 0.1f)
        {
            Vector3 point = origin + direction * d;
            int bx = (int)Math.Floor(point.X);
            int by = (int)Math.Floor(point.Y);
            int bz = (int)Math.Floor(point.Z);
            BlockType block = _world.GetBlock(bx, by, bz);
            if (block != BlockType.Air && block != BlockType.Water)
            {
                return (new Vector3(bx, by, bz), new Vector3((int)Math.Floor(previous.X), (int)Math.Floor(previous.Y), (int)Math.Floor(previous.Z)));
            }
            previous = point;
        }
        return (null, null);
    }

    /// <summary>
    /// Формирует текст с координатами игрока и подсказками по управлению.
    /// </summary>
    private string BuildHud(BlockType? target)
    {
        Vector3 pos = _player.Position;
        string targetText = target?.ToString() ?? "None";
        string controls = "WASD: движение | Стрелки: взгляд | Space: прыжок | Shift: бег | E: ломать | Q: ставить | Esc: выход";
        return $"Pos: {pos.X:F1},{pos.Y:F1},{pos.Z:F1}  Block: {targetText}  {controls}\n";
    }
}
