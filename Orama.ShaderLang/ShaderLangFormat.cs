namespace Orama.ShaderLang;

/// <summary>
/// Data representation of an Orama shader format.
/// </summary>
public class ShaderLangFormat
{
    /// <summary> Parse a shader from a string. </summary>
    public static ShaderLangFormat FromSource(string source)
    {
        return new ShaderLangFormat();
    }

    /// <summary> The pass this shader belongs to. </summary>
    public string? Pass { get; set; }
}
