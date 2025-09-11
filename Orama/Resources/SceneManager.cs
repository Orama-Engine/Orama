using Orama.Components;
using Orama.Entities;
using Orama.Resources;

namespace Orama.Resources;

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

		// Start all entities (this also starts their components)
		foreach (var entity in Current.AllEntities)
			entity.Start();
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
		// Update all entities (this also updates their components)
		foreach(var entity in Current.AllEntities)
			entity.Update();
    }

	public static void LoadScene(Scene scene)
	{
		Clear();
		Current = scene;
	}
}