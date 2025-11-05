using Orama.Core.Modules.GUI.Styling;
using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;

namespace Orama.Core.Modules.GUI.Widgets;

// TODO: Discuss moving this widget to the rendering module

/// <summary>
/// A Widget used to display a <see cref="Texture"/>
/// </summary>
public class Display : Widget
{
    /// <summary> The texture to display. </summary>
    public Texture? Texture { get; set; }

    /// <inheritdoc/>
    public override Vector2 SizeHint => new Vector2(Texture?.Width ?? 0, Texture?.Height ?? 0);

    /// <inheritdoc/>
    public override void Draw(Style style)
    {
        if (Texture == null)
            return;

        Rect refRect = WorldRect;
        PaintEngine.DrawTexture(ref refRect, Texture);
    }
}
