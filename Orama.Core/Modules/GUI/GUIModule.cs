using Orama.Core.Common.Utility;
using Orama.Core.Modules.Rendering;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Module responsible for handling GUI.
/// </summary>
public class GUIModule : BaseModule
{
    public override HashSet<Type> Dependencies { get; } = new() { typeof(RenderingModule) };

    public override void Update()
    {
        Rect guiRect = new(0, 0, 400, 400);
        Drawer.DrawRect(ref guiRect, new(1, 1, 1, 1));
    }
}
