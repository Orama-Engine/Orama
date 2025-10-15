
namespace Orama.Core.Common.Utility;

/// <summary>
/// Utility for outputting to the engine console.
/// </summary>
public static class EngineOutput
{
    /// <summary> Output an exception. </summary>
    public static void Exception(Exception ex)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;

        Console.Error.WriteLine("[Exception] " + ex);

        Console.ForegroundColor = prevColor;
    }

    /// <summary> Output a message. </summary>
    public static void WriteLine(string message)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;

        Console.WriteLine("[Log] " + message);

        Console.ForegroundColor = prevColor;
    }
}
