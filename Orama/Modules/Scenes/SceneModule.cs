using Orama.Components;
using Orama.Entities;
using Orama.Resources;

namespace Orama.Modules.Scenes;

/// <summary>
/// Manages Scenes and their operations.
/// </summary>
public class SceneModule : Module
{
	/// <summary> The currently active Scene. </summary>
    public Scene Current { get; private set; } = null!;
	
    public override void Start()
	{
		NewScene();
	}

	/// <summary> Creates a new Scene. </summary>
	public void NewScene()
	{
		Clear();
		Current = new Scene();

		// Start all entities (this also starts their components)
		foreach (var entity in Current.AllEntities)
			entity.Start();
	}

	/// <summary> Clears the current Scene. </summary>
	public void Clear()
    {
        if (Current != null)
        {
            Current.Clear();
            Current = new Scene();
        }
    }
	
	public override void Update()
    {
		// Update all entities (this also updates their components)
		foreach(var entity in Current.AllEntities)
			entity.Update();
    }

	public void LoadScene(Scene scene)
	{
		Clear();
		Current = scene;

		// Start all entities (this also starts their components)
		foreach (var entity in Current.AllEntities)
			entity.Start();
	}
}