using Orama.ShaderLang.Parser;

namespace Orama.ShaderLang;

/// <summary>
/// Data representation of an Orama shader format.
/// </summary>
public class ShaderLangFormat
{
    /// <summary> Parse a shader from a string. </summary>
    public static ShaderLangFormat FromSource(string source)
    {
        var lexer = new Lexer(source);
        var parser = new Parser.Parser(lexer.Tokenize());
        return parser.Parse();
    }

    /// <summary> Metadata defined in the shader. </summary>
    public Dictionary<string, string> MetaData { get; set; } = new Dictionary<string, string>();
    
    /// <summary> The name of the shader. </summary>
    public string? Name { get; set; }

    /// <summary> The pass this shader belongs to. </summary>
    public string? Pass { get; set; }

}
