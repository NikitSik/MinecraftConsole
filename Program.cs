namespace MinecraftConsole;

/// <summary>
/// Точка входа консольного приложения. Создаёт игру и запускает главный цикл.
/// </summary>
public static class Program
{
    /// <summary>
    /// Запускает игровой процесс. Здесь создаётся единственный экземпляр игры.
    /// </summary>
    public static void Main()
    {
        Game game = new();
        game.Run();
    }
}
