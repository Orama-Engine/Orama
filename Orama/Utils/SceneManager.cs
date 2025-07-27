using Orama.Resources;

namespace Orama.Utils;

/// <summary>
/// Manages Scenes and their operations.
/// </summary>
public static class SceneManager
{
	/// <summary> The currently active Scene. </summary>
    public static Scene Current { get; private set; } = new Scene();
    
    /// <summary> Initialize the Scene Manager. </summary>
    public static void Initialize() { }

    /// <summary> Creates a new Scene. </summary>
    public static void NewScene()
    {
        Clear();
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
    }
}