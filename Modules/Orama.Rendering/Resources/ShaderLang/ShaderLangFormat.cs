using Orama.ShaderLang.Parser;

namespace Orama.ShaderLang;

/// <summary>
/// Data representation of an Orama shader format.
/// </summary>
public class ShaderLangFormat
{
    /// <summary> Metadata defined in the shader. </summary>
    public Dictionary<string, string> MetaData { get; set; } = new Dictionary<string, string>();
    
    /// <summary> The name of the shader. </summary>
    public string? Name { get; set; }

    /// <summary> The pass this shader belongs to. </summary>
    public string? Pass { get; set; }

    /// <summary> The properties of the shader. </summary>
    public ShaderLangProperties? Properties { get; set; }

    /// <summary> The HLSL source code of the shader. </summary>
    public ShaderLangSource? Source { get; set; }

    /// <summary> Parse a shader from a string. </summary>
    public static ShaderLangFormat FromSource(string source)
    {
        var lexer = new Lexer(source);
        var parser = new Parser.Parser(lexer.Tokenize());
        return parser.Parse();
    }

}

/// <summary>
/// Data representation of a shader property block.
/// </summary>
public class ShaderLangProperties
{
    /// <summary> The body of the property block. </summary>
    public string? Body { get; set; }
}

/// <summary>
/// Data representation of a shader source block.
/// </summary>
public class ShaderLangSource
{
    /// <summary> The body of the source block. </summary>
    public string? Body { get; set; }
}