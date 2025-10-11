
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
}
