
namespace Orama.Audio;

public static class AudioManager
{
	public static Dictionary<string, uint> SourceCache = new();
	public static Dictionary<string, uint> BufferCache = new();

	/// <summary>
	/// Plays the sound at the specified file path.
	/// </summary>
	/// <param name="filePath"></param>
	public static void PlaySound(string filePath)
	{
		// Normalise the path before using it as a key for the caches
		string key = Path.GetFullPath(filePath).ToLowerInvariant();
		
		// Parse the file and create buffer if not yet loaded
		if (!BufferCache.ContainsKey(filePath))
		{
			var wav = WavParser.FromFile(filePath);

			var format = wav.GetFormat();
			var buffer = AudioBackend.CreateBuffer(wav.PcmData, format, wav.PcmData.Length, wav.SampleRate);
			BufferCache[key] = buffer;
		}

		// Stops and deletes the source if it already exists
		if (SourceCache.TryGetValue(key, out var prevSource))
		{
			AudioBackend.StopSource(prevSource);
			AudioBackend.DeleteSource(prevSource);
		}

		var source = AudioBackend.GenerateSource();
		AudioBackend.AttachBufferToSource(source, BufferCache[key]);
		
		// Play the sound
		AudioBackend.PlaySource(source);
		
		SourceCache[key] = source;
	}

	/// <summary>
	/// Stops the sound at the specified file path if it's playing.
	/// </summary>
	/// <param name="filePath"></param>
	public static void StopSound(string filePath)
	{
		string key = Path.GetFullPath(filePath).ToLowerInvariant();
		
		if (SourceCache.TryGetValue(key, out var source))
		{
			AudioBackend.StopSource(source);
			AudioBackend.DeleteSource(source);
		}
	}
}