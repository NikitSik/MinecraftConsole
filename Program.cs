namespace MinecraftConsole;

/// <summary>
/// Entry point for the console Minecraft clone.
/// </summary>
public static class Program
{
    public static void Main()
    {
        Game game = new();
        game.Run();
    }
}
