using Orama.Core.Common.Entities;
using Orama.Core.Common.Utility;

namespace Orama.Core.Modules.Scenes.Resources;

/// <summary>
/// A collection of Entities.
/// </summary>
public class Scene
{
    /// <summary> All Entities in the Scene. </summary>
    public List<Entity> Entities { get; set; } = new();

    public void StartAll()
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
                EngineOutput.Exception(ex);
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
                EngineOutput.Exception(ex);
            }
        }
    }
}
