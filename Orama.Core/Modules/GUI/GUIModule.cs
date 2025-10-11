using Orama.Core.Common;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Rendering;
using Orama.Math;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Module responsible for drawing and managing GUI Widgets.
/// </summary>
public class GUIModule : BaseModule
{
    public override HashSet<Type> Dependencies => new() { typeof(RenderingModule) };

    /// <summary> All widgets that are being drawn. </summary>
    public List<BaseWidget> Widgets { get; set; } = new();

    private Matrix4x4 ortho => Matrix4x4.CreateOrthographicOffCenter(
        0, Application.Window.Size.X,
        0, Application.Window.Size.Y,
        -1, 1
    );

    public override void Initialize()
    {
        Application.OnRender += Draw;
    }


    public void Draw()
    {
        var rendering = ModuleManager.GetModule<RenderingModule>();

        // Temporarily override the camera matrices for GUI rendering
        rendering?.ViewOverride = Matrix4x4.Identity;
        rendering?.ProjectionOverride = ortho;

        foreach (var widget in Widgets)
            widget.Paint();

        // Reset overrides
        rendering?.ViewOverride = null;
        rendering?.ProjectionOverride = null;
    }
}
