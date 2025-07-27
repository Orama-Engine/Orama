using Orama.Math;
using Orama.Rendering;
using Orama.Resources.ResourceLibrary;
using Orama.Utils;

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

	public static event Action Initialize = null!;
	public static event Action Update = null!;
	public static event Action Render = null!;
	public static event Action Quitting = null!;

	public static void Run(string title, int width, int height, IResourceLibrary resourceLibrary)
    {
		ResourceLibrary = resourceLibrary;

		Window.load += AppInitialize;
		Window.update += AppUpdate;
		Window.closing += AppClose;

		Window.Start(title, new Vector2I(width, height), new Vector2I(100, 100));
	}

	public static void AppInitialize()
	{
		Initialize?.Invoke();
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
