
namespace Orama.Core.Common.Utility;

/// <summary>
/// Utility for outputting to the engine console.
/// </summary>
public static class EngineOutput
{
    /// <summary> The current console output. </summary>
    public static List<string> Output { get; set; } = new();

    /// <summary> Output an exception. </summary>
    public static void Exception(Exception ex)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;

        string output = "[Exception] " + ex;
        Console.Error.WriteLine(output);

        Console.ForegroundColor = prevColor;

        Output.Add(output);
    }

    /// <summary> Output a message. </summary>
    public static void Log(string message)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Gray;

        string output = "[Log] " + message;
        Console.WriteLine(output);

        Console.ForegroundColor = prevColor;

        Output.Add(output);
    }

    /// <summary> Output a warning. </summary>
    public static void Warning(string message)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;

        string output = "[Warning] " + message;
        Console.WriteLine(output);

        Console.ForegroundColor = prevColor;

        Output.Add(output);
    }
}
