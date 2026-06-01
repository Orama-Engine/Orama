
namespace Orama.Math;

/// <summary>
/// Provides common math utility.
/// </summary>
public static class Math
{
    /// <summary> Returns the smaller of the two values. </summary>
    public static int Min(int a, int b) => a < b ? a : b;

    /// <summary> Returns the greater of the two values. </summary>
    public static int Max(int a, int b) => a > b ? a : b;

    /// <summary> Clamps a value between min and max. </summary>
    public static float Clamp(float value, float min, float max) => MathF.Min(MathF.Max(value, min), max);
}
