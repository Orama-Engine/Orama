
namespace Orama.Core.Modules.GUI;

/// <summary>
/// Represents a two-dimensional rectangle.
/// </summary>
public struct Rect
{
    /// <summary> The X coordinate of the top-left corner of the rectangle. </summary>
    public int X { get; set; }

    /// <summary> The Y coordinate of the top-left corner of the rectangle. </summary>
    public int Y { get; set; }

    /// <summary> The width of the rectangle. </summary>
    public int Width { get; set; }

    /// <summary> The height of the rectangle. </summary>
    public int Height { get; set; }

    /// <summary> Initializes a new instance of the <see cref="Rect"/> class with the specified coordinates and dimensions. </summary>
    public Rect(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}
