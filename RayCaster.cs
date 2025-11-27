using System.Numerics;
using System.Text;

namespace MinecraftConsole;

/// <summary>
/// Simple console ray-caster that turns the voxel world into vertical columns of ASCII symbols.
/// </summary>
public class RayCaster
{
    public const int ScreenWidth = 120;
    public const int ScreenHeight = 40;
    public const float FieldOfView = 70f * (float)Math.PI / 180f;
    public const float MaxRenderDistance = 32f;

    private readonly char[,] _buffer = new char[ScreenHeight, ScreenWidth];

    public char[,] Buffer => _buffer;

    public void Render(World world, Player player, out BlockType? targetBlock)
    {
        for (int y = 0; y < ScreenHeight; y++)
        for (int x = 0; x < ScreenWidth; x++)
            _buffer[y, x] = ' ';

        targetBlock = null;

        for (int x = 0; x < ScreenWidth; x++)
        {
            float cameraX = (2f * x / ScreenWidth - 1f) * (FieldOfView * 0.5f);
            float rayAngle = player.Yaw + cameraX;
            Vector3 dir = new(
                (float)(Math.Sin(rayAngle) * Math.Cos(player.Pitch)),
                (float)Math.Sin(player.Pitch),
                (float)(Math.Cos(rayAngle) * Math.Cos(player.Pitch)));

            float distance = Cast(world, player.Position, dir, out BlockType hitBlock);
            if (distance < MaxRenderDistance && hitBlock != BlockType.Air)
            {
                if (targetBlock == null) targetBlock = hitBlock;
                DrawColumn(x, distance, hitBlock, player.Pitch);
            }
            else
            {
                DrawSkyAndGround(x, player.Pitch);
            }
        }
    }

    private static float Cast(World world, Vector3 origin, Vector3 direction, out BlockType hit)
    {
        float step = 0.1f;
        for (float d = 0f; d < MaxRenderDistance; d += step)
        {
            Vector3 point = origin + direction * d;
            int bx = (int)Math.Floor(point.X);
            int by = (int)Math.Floor(point.Y);
            int bz = (int)Math.Floor(point.Z);
            BlockType block = world.GetBlock(bx, by, bz);
            if (block != BlockType.Air && block != BlockType.Water)
            {
                hit = block;
                return d;
            }
        }

        hit = BlockType.Air;
        return MaxRenderDistance;
    }

    private void DrawColumn(int column, float distance, BlockType block, float pitch)
    {
        float projectionHeight = ScreenHeight / (distance + 0.0001f);
        int columnHeight = (int)Math.Clamp(projectionHeight, 1, ScreenHeight);
        int verticalShift = (int)(pitch * ScreenHeight * 0.4f);
        int ceiling = ScreenHeight / 2 - columnHeight / 2 - verticalShift;
        int floor = ceiling + columnHeight;

        for (int y = 0; y < ScreenHeight; y++)
        {
            if (y < ceiling)
            {
                _buffer[y, column] = ' ';
            }
            else if (y >= ceiling && y <= floor && y >= 0 && y < ScreenHeight)
            {
                char shade = Shade(block, distance);
                _buffer[y, column] = shade;
            }
            else
            {
                _buffer[y, column] = '.';
            }
        }
    }

    private void DrawSkyAndGround(int column, float pitch)
    {
        int horizon = ScreenHeight / 2 - (int)(pitch * ScreenHeight * 0.4f);
        for (int y = 0; y < ScreenHeight; y++)
        {
            _buffer[y, column] = y < horizon ? ' ' : '.';
        }
    }

    private static char Shade(BlockType block, float distance)
    {
        char symbol = block switch
        {
            BlockType.Grass => '^',
            BlockType.Stone => '#',
            BlockType.Dirt => '%',
            BlockType.Sand => '~',
            BlockType.Water => '~',
            _ => '#'
        };

        float depth = distance / MaxRenderDistance;
        if (depth < 0.2f) return symbol;
        if (depth < 0.4f) return symbol switch { '^' => '"', '#' => '@', '%' => '&', '~' => '~', _ => '+' };
        if (depth < 0.6f) return '+';
        if (depth < 0.8f) return '*';
        return '.';
    }

    public string ComposeFrame(string hud)
    {
        StringBuilder sb = new(ScreenHeight * (ScreenWidth + 1));
        for (int y = 0; y < ScreenHeight; y++)
        {
            for (int x = 0; x < ScreenWidth; x++)
                sb.Append(_buffer[y, x]);
            sb.Append('\n');
        }
        sb.Append(hud);
        return sb.ToString();
    }
}
