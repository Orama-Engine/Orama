
namespace Orama.Math;

/// <summary>
/// Represents a color.
/// </summary>
public struct Color
{
    /// <summary> The red component of the color. </summary>
    public float R { get; set; }

    /// <summary> The green component of the color. </summary>
    public float G { get; set; }

    /// <summary> The blue component of the color. </summary>
    public float B { get; set; }

    /// <summary> The alpha (transparency) component of the color. </summary>
    public float A { get; set; }

    /// <summary> Creates a new instance of <see cref="Color"/> with the specified components. </summary>
    public Color(float r, float g, float b, float a) => (R, G, B, A) = (r, g, b, a);

    public static explicit operator Vector4(Color color) => new Vector4(color.R, color.G, color.B, color.A);
}
