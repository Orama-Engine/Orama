
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

        Console.WriteLine("[Exception] " + ex);

        Console.ForegroundColor = prevColor;
    }
}
