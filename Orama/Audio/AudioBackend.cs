using Silk.NET.OpenAL;

namespace Orama.Audio;

/// <summary>
/// Handles low level OpenAL stuff.
/// </summary>
public static class AudioBackend
{
	public static Dictionary<int, uint> Sources = new();
		
	private static ALContext? alContextApi;
	private static AL? al;
	
	// Raw handles
	private static unsafe Device* device;
	private static unsafe Context* context;

	/// <summary>
	/// Initializes the audio device, creates an OpenAL API context.
	/// </summary>
	public static unsafe void Initialize()
	{
		alContextApi = ALContext.GetApi();
		
		// Open the default device
		device = alContextApi.OpenDevice(null);
		if (device == null)
		{
			Console.WriteLine("Failed to open device");
			return;
		}
		
		// Create context with default attributes
		context = alContextApi.CreateContext(device, null);

		if (context == null)
		{
			Console.WriteLine("Failed to create context");
			return;
		}
		
		// Make context current
		bool madeCurrent = alContextApi.MakeContextCurrent(context);
		if (!madeCurrent)
		{
			Console.WriteLine("Failed to create context");
			return;
		}
		
		// Get AL Api instance
		al = AL.GetApi();
		
		Console.WriteLine("Audio device initialized successfully.");
	}

	/// <summary>
	/// De-initializes the audio device, releases resources.
	/// </summary>
	public static unsafe void Shutdown()
	{
		if (alContextApi != null)
		{
			alContextApi.MakeContextCurrent(null);
			alContextApi.Dispose();
			alContextApi = null;
		}
		
		if (device != null)
		{
			alContextApi?.CloseDevice(device);
			device = null;
		}
		
		Console.WriteLine("Audio device shutdown successfully.");
	}

	/// <summary>
	/// Creates an OpenAL buffer, fills it with audio data, and returns its handle.
	/// </summary>
	/// <param name="data"></param>
	/// <param name="format"></param>
	/// <param name="size"></param>
	/// <param name="sampleRate"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Thrown if the audio system is not initialised.</exception>
	public static unsafe uint CreateBuffer(byte[] data, BufferFormat format, int size,int sampleRate)
	{
		if (al == null)
			throw new InvalidOperationException("Audio not initialized.");

		uint buffer = al.GenBuffer();

		fixed(byte* pData = data)
			al.BufferData(buffer, format, pData, size, sampleRate);

		return buffer;
	}

	/// <summary>
	/// Deletes an OpenAL buffer, freeing its resources.
	/// </summary>
	/// <param name="buffer"></param>
	/// <exception cref="InvalidOperationException">Thrown if the audio system is not initialised.</exception>
	public static void DeleteBuffer(uint buffer)
	{
		if (al == null)
			throw new InvalidOperationException("Audio not initialized.");
		
		al.DeleteBuffer(buffer);
	}

	/// <summary>
	/// Generates an audio source.
	/// </summary>
	/// <param name="id"></param>
	/// <returns>Audio Source.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the audio system is not initialized, source has the same ID or generation fails.</exception>
	public static uint GenerateSource(int id)
	{
		if (al == null)
			throw new InvalidOperationException("Audio not initialized.");
		
		if (Sources.ContainsKey(id))
			throw new InvalidOperationException("Source Id already registered.");
		
		var source = al.GenSource();
		if (source == 0)
			throw new InvalidOperationException("Failed to generate source.");

		Sources.Add(id, source);
		return source;
	}

	/// <summary>
	/// Attaches an OpenAL buffer to the current audio source for playback.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="buffer"></param>
	/// <exception cref="InvalidOperationException">Thrown if the audio system is not initialized or there is no audio source.</exception>
	public static void AttachBufferToSource(uint source, uint buffer)
	{
		if (al == null || !Sources.ContainsValue(source))
			throw new InvalidOperationException("Audio not initialized or there is no audio source available.");

		al.SetSourceProperty(source, SourceInteger.Buffer, buffer);
	}
}