// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Runtime.CompilerServices;

namespace Orama.Common.Utility;

/// <summary>
/// Utility for outputting to the engine console.
/// </summary>
public static class EngineConsole
{
	/// <summary> The current console output. </summary>
	public static List<string> Output { get; set; } = new();

	/// <summary> Output an exception. </summary>
	public static void Exception(Exception ex, [CallerFilePath] string origin = "Exception", [CallerMemberName] string member = "Unknown")
	{
		var prevColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Red;

		ReadOnlySpan<char> parsedOrigin = Path.GetFileNameWithoutExtension(origin.AsSpan());

		string output = $"[Exception ({parsedOrigin}.{member})] {ex.Message}";
		Console.Error.WriteLine(output);

		Console.ForegroundColor = prevColor;

		Output.Add(output);
	}

	/// <summary> Output a message. </summary>
	public static void Log(string message, [CallerFilePath] string origin = "Log")
	{
		var prevColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Gray;

		ReadOnlySpan<char> parsedOrigin = Path.GetFileNameWithoutExtension(origin.AsSpan());

		string output = $"[{parsedOrigin}] {message}";
		Console.WriteLine(output);

		Console.ForegroundColor = prevColor;

		Output.Add(output);
	}

	/// <summary> Output a warning. </summary>
	public static void Warning(string message, [CallerFilePath] string origin = "Warning", [CallerMemberName] string member = "Unknown")
	{
		var prevColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Yellow;

		ReadOnlySpan<char> parsedOrigin = Path.GetFileNameWithoutExtension(origin.AsSpan());

		string output = $"[Warning ({parsedOrigin}.{member})] {message}";
		Console.WriteLine(output);

		Console.ForegroundColor = prevColor;

		Output.Add(output);
	}
}
