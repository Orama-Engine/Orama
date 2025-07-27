using Orama.Math;
using Orama.Resources;

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

    public static void Run(IResourceLibrary resourceLibrary)
    {
		ResourceLibrary = resourceLibrary;

		TextAsset ta = ResourceLibrary.GetResource<TextAsset>("DefaultAssets/test.txt");
		ta.Content = "Output!";

		ResourceLibrary.WriteResource("DefaultAssets/test.txt", ta);

	}
}
