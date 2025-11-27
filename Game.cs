using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace MinecraftConsole;

/// <summary>
/// Coordinates initialization, the main loop, updates and rendering.
/// </summary>
public class Game
{
    private const string SavePath = "world.save";
    private const int FrameTimeMs = 33; // ~30 FPS

    private readonly Player _player = new();
    private readonly InputHandler _input = new();
    private readonly RayCaster _rayCaster = new();
    private readonly SaveSystem _saveSystem = new();
    private World _world = new();

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

    private bool HandleExit()
    {
        Console.SetCursorPosition(0, RayCaster.ScreenHeight + 1);
        Console.Write("Save world before exiting? (Y/N): ");
        ConsoleKey key = Console.ReadKey(true).Key;
        if (key == ConsoleKey.Y)
        {
            _saveSystem.SaveWorld(_world, SavePath);
        }
        return true;
    }

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

    private string BuildHud(BlockType? target)
    {
        Vector3 pos = _player.Position;
        string targetText = target?.ToString() ?? "None";
        string controls = "WASD: move | Arrows: look | Space: jump | Shift: sprint | E: break | Q: place | Esc: exit";
        return $"Pos: {pos.X:F1},{pos.Y:F1},{pos.Z:F1}  Block: {targetText}  {controls}\n";
    }
}
