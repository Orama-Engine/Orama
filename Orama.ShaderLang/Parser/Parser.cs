
namespace Orama.ShaderLang.Parser;

internal class Parser
{
    private readonly List<Token> tokens;
    private int pos = 0;

    public Parser(List<Token> input)
    {
        tokens = input;
    }

    /// <summary> Parses the tokens into a <see cref="ShaderLangFormat"/> instance. </summary>
    public ShaderLangFormat Parse()
    {
        var shader = new ShaderLangFormat();

        while (!IsAtEnd())
        {
            var token = Peek();

            if (token.Type == TokenType.Identifier)
            {
                if (token.Value.Equals("pass", StringComparison.OrdinalIgnoreCase))
                {
                    Advance();
                    ParsePass(shader);
                    continue;
                }
            }
            else if (token.Type == TokenType.Hash)
            {
                Advance();
                ParseMetaData(shader);
                continue;
            }

            Advance();
        }

        return shader;
    }

    private void ParsePass(ShaderLangFormat shader)
    {
        if (Check(TokenType.Equals))
            Advance();

        if (Check(TokenType.String) || Check(TokenType.Identifier))
        {
            shader.Pass = Advance().Value;
        }
    }


    private void ParseMetaData(ShaderLangFormat shader)
    {
        if (!Check(TokenType.Identifier))
            return;

        string key = Advance().Value;

        if (Check(TokenType.Equals))
            Advance();

        if (Check(TokenType.String) || Check(TokenType.Identifier))
        {
            string value = Advance().Value;
            shader.MetaData[key] = value;
        }
    }

    private Token Peek() => tokens[pos];
    private Token Advance() => tokens[pos++];
    private bool Check(TokenType type) => !IsAtEnd() && tokens[pos].Type == type;
    private bool IsAtEnd() => pos >= tokens.Count || tokens[pos].Type == TokenType.EndOfFile;
}
