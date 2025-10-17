
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

    /// <summary> Initializes a new instance of <see cref="Color"/> with the specified components. </summary>
    public Color(float r, float g, float b, float a) => (R, G, B, A) = (r, g, b, a);
    #region Predefined Colors
    public static Color Transparent => new Color(0, 0, 0, 0);
    public static Color White => new Color(1, 1, 1, 1);
    public static Color Black => new Color(0, 0, 0, 1);
    public static Color Red => new Color(1, 0, 0, 1);
    public static Color Green => new Color(0, 1, 0, 1);
    public static Color Blue => new Color(0, 0, 1, 1);
    #endregion

    /// <summary> Creates a <see cref="Color"/> from 0-255 values. </summary>
    public static Color From255(int r, int g, int b, int a) => new Color(r / 255f, g / 255f, b / 255f, a / 255f);

    public static explicit operator Vector4(Color color) => new Vector4(color.R, color.G, color.B, color.A);

    public static Color operator *(Color color, float scalar) => new Color(color.R * scalar, color.G * scalar, color.B * scalar, color.A * scalar);
}
