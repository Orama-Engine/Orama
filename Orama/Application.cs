using System.Numerics;
using Orama.Rendering;
using Orama.Rendering.Materials;
using Orama.Resources.ResourceLibrary;

namespace Orama;

/// <summary>
/// Main Application.
/// </summary>
public static class Application
{
	/// <summary>
	/// Resource Management.
	/// </summary>
	public static IResourceLibrary ResourceLibrary { get; set; } = null!;

	public static event Action? Initialize;
	public static event Action? Update;
	public static event Action? Render;
	public static event Action? Quitting;

	public static void Run(string title, int width, int height, IResourceLibrary resourceLibrary)
    {
		ResourceLibrary = resourceLibrary;

		Window.load += AppInitialize;
		Window.update += AppUpdate;
		Window.closing += AppClose;
		
		Window.Start(title, new Vector2(width, height), new Vector2(100, 100));
	}

	public static void AppInitialize()
	{
		Initialize?.Invoke();

		Material material = Material.Default;
		Stream stream = Serialization.Serializer.Serialize(material);
		using var reader = new StreamReader(stream);
		Console.WriteLine(reader.ReadToEnd());
	}

	public static void AppUpdate()
	{
		try
		{
			Update?.Invoke();
			Render?.Invoke();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
		
	}

	public static void AppClose()
	{

	}
}
