using Orama.Core.Common.Entities;

namespace Orama.Core.Modules.Scenes.Resources;

/// <summary>
/// A collection of Entities.
/// </summary>
public class Scene
{
    /// <summary> All Entities in the Scene. </summary>
    public List<Entity> Entities { get; set; } = new();

    internal void StartAll()
    {
        foreach (var entity in Entities)
        {
            if (!entity.Enabled)
                continue;

            try
            {
                entity.Start();
            }
            catch (Exception ex)
            {
                LogEntityException("Start", entity, ex);
            }
        }
    }

    internal void UpdateAll()
    {
        foreach (var entity in Entities)
        {
            if (!entity.Enabled)
                continue;

            try
            {
                entity.Update();
            }
            catch (Exception ex)
            {
                LogEntityException("Update", entity, ex);
            }
        }
    }

    private static void LogEntityException(string method, Entity entity, Exception ex)
    {
        string entityType = entity.GetType().FullName ?? "UnknownEntity";
        string assemblyName = entity.GetType().Assembly.GetName().Name ?? "UnknownAssembly";

        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine($"[Exception] ({assemblyName}) {entityType}.{method} threw an exception: {ex}");

        Console.ForegroundColor = prevColor;
    }
}
