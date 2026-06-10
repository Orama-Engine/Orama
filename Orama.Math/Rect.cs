using System.Runtime.CompilerServices;

namespace Orama.Math;

/// <summary>
/// Represents a two-dimensional rectangle.
/// </summary>
public struct Rect
{
    /// <summary> The X position of the <see cref="Rect"/>. </summary>
    public int X {  get; set; }

    /// <summary> The Y position of the <see cref="Rect"/>. </summary>
    public int Y {  get; set; }

    /// <summary> The width (X scale) of the <see cref="Rect"/>. </summary>
    public int Width {  get; set; }

    /// <summary> The height (Y scale) of the <see cref="Rect"/>. </summary>
    public int Height {  get; set; }

    /// <summary> Creates a new instance of <see cref="Rect"/>. </summary>
    public Rect() => (X, Y, Width, Height) = (0, 0, 0, 0);

    /// <summary> Creates a new instance of <see cref="Rect"/> with the specified components. </summary>
    public Rect(int x, int y) => (X, Y, Width, Height) = (x, y, 0, 0);

    /// <summary> Creates a new instance of <see cref="Rect"/> with the specified components. </summary>
    public Rect(int x, int y, int width, int height) => (X, Y, Width, Height) = (x, y, width, height);

    /// <summary> Checks if the specified coordinates are contained in the <see cref="Rect"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(int x, int y) => x >= X && x < X + Width && y >= Y && y < Y + Height;
}
