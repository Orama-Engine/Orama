using Orama.Resources;

namespace Orama.Utils;

/// <summary>
/// Manages Scenes and their operations.
/// </summary>
public static class SceneManager
{
    public static Scene Current { get; private set; } = new Scene();
    public static Scene Scene => Current;
    
    public static void Initialize() { }

    public static void NewScene()
    {
        Clear();
    }

    public static void Clear()
    {
        if (Current != null)
        {
            Current.Clear();
            Current = new Scene();
        }
    }

    public static void Update()
    {
    }
}