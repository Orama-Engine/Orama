
namespace Orama.Core.Modules.GUI;

/// <summary>
/// Represents a 2D rectangle.
/// </summary>
public struct Rect
{
    /// <summary> The X coordinate of the top-left corner of the rectangle. </summary>
    public float X { get; set; }

    /// <summary> The Y coordinate of the top-left corner of the rectangle. </summary>
    public float Y { get; set; }

    /// <summary> The width of the rectangle. </summary>
    public float Width { get; set; }

    /// <summary> The height of the rectangle. </summary>
    public float Height { get; set; }

    /// <summary> Initializes a new instance of the <see cref="Rect"/> struct. </summary>
    public Rect(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <inheritdoc/>
    override public string ToString() => $"X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
}
