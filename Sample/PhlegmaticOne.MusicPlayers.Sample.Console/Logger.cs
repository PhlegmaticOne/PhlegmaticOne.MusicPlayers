namespace PhlegmaticOne.MusicPlayers.Sample.Console;

public static class Logger
{
    public static Action<object?> LogTo { get; set; } = System.Console.WriteLine;
    public static Action ClearAction { get; set; } = System.Console.Clear;
    public static void Log(object? obj) => LogTo(obj);
    public static void Clear() => ClearAction();
}