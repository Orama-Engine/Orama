using Orama.Core.Common.Entities;

namespace Orama.Core.Modules.Scenes.Resources;

/// <summary>
/// A collection of Entities.
/// </summary>
public class Scene
{
    /// <summary> All Entities in the Scene. </summary>
    public List<BaseEntity> Entities { get; set; } = new();

    internal void StartAll()
    {
        foreach (var entity in Entities)
        {
            if (entity.Enabled)
                entity.Start();
        }
    }

    internal void UpdateAll()
    {
        foreach (var entity in Entities)
        {
            if (entity.Enabled)
                entity.Update();
        }
    }
}
