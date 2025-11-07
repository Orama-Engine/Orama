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
        var shader = new ShaderLangFormat();

        var lexer = new Lexer(source);
        var tokens = lexer.Tokenize();

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            if (token.Type == TokenType.Identifier && token.Value.Equals("pass", StringComparison.OrdinalIgnoreCase))
            {
                int valueIndex = i + 1;
                if (valueIndex < tokens.Count && tokens[valueIndex].Type == TokenType.Equals)
                    valueIndex++;

                if (valueIndex < tokens.Count && (tokens[valueIndex].Type == TokenType.String || tokens[valueIndex].Type == TokenType.Identifier))
                {
                    shader.Pass = tokens[valueIndex].Value;
                    i = valueIndex;
                }
            }
        }

        return shader;
    }

    /// <summary> The pass this shader belongs to. </summary>
    public string? Pass { get; set; }
}
