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
