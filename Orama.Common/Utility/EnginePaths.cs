namespace Orama.Common.Utility;

/// <summary>
/// Utility for using file paths relative to the engine.
/// </summary>
public static class EnginePaths
{
    /// <summary> The path to the engine's assembly directory. </summary>
    public static string EngineDirectory => Path.GetDirectoryName(typeof(EnginePaths).Assembly.Location) ?? string.Empty;
}
