using Orama.Components;
using Orama.Core;
using Orama.Resources;

namespace Orama.Engine;

/// <summary>
/// Manages Scenes and their operations.
/// </summary>
public static class SceneManager
{
	/// <summary> The currently active Scene. </summary>
    public static Scene Current { get; private set; } = null!;
    
    /// <summary> Initialize the Scene Manager. </summary>
    public static void Initialize()
	{
		NewScene();
	}

	/// <summary> Creates a new Scene. </summary>
	public static void NewScene()
	{
		Clear();
		Current = new Scene();

		// Create new entities
		Entity mesh = new Entity();
		mesh.AddComponent(new MeshRenderer());
		mesh.AddComponent(new CameraController());
		Current.Add(mesh);

		// Start all components
		foreach (var entity in Current.AllEntities)
		{
			foreach (var component in entity.Components)
			{
				component.Start();
			}
		}
	}

	/// <summary> Clears the current Scene. </summary>
	public static void Clear()
    {
        if (Current != null)
        {
            Current.Clear();
            Current = new Scene();
        }
    }

    /// <summary> Runs each frame. </summary>
    public static void Update()
    {
		foreach(var entity in Current.AllEntities)
		{
			foreach(var component in entity.Components)
			{
				component.Update();
			}
		}
    }
}