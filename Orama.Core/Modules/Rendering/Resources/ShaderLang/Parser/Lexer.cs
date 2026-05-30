
using System.Text;

namespace Orama.ShaderLang.Parser;

internal class Lexer
{
    private readonly string source;
    private int pos = 0;

    /// <summary> Initializes a new instance of the <see cref="Lexer"/> class. </summary>
    public Lexer(string source)
    {
        this.source = source;
    }

    /// <summary> Converts the source code into a list of tokens. </summary>
    public List<Token> Tokenize()
    {
        List<Token> tokens = new List<Token>();

        while (!IsAtEnd())
        {
            char c = Peek();

            if (char.IsWhiteSpace(c))
            {
                Advance();
                continue;
            }

            if (c == '{')
            {
                tokens.Add(ReadBlock());
                continue;
            }

            if (c == '=')
            {
                tokens.Add(new Token(TokenType.Equals, "="));
                Advance();
                continue;
            }

            if (c == '#')
            {
                tokens.Add(new Token(TokenType.Hash, "#"));
                Advance();
                continue;
            }

            if (c == '"')
            {
                tokens.Add(ReadString());
                continue;
            } 
            else if (char.IsDigit(c))
            {
                tokens.Add(ReadNumber());
                continue;
            } 
            else
            {
                tokens.Add(ReadIdentifier());
                continue;
            }

            throw new Exception($"Unexpected character '{c}'");
        }

        tokens.Add(new Token(TokenType.EndOfFile, ""));
        return tokens;
    }

    private Token ReadBlock()
    {
        Advance();

        StringBuilder sb = new StringBuilder();
        int braceCount = 1;

        while (!IsAtEnd() && braceCount > 0)
        {
            char c = Advance();

            if (c == '{')
            {
                braceCount++;
                sb.Append(c);
            }
            else if (c == '}')
            {
                braceCount--;
                if (braceCount > 0)
                    sb.Append(c);
            }
            else
            {
                sb.Append(c);
            }
        }

        return new Token(TokenType.Block, sb.ToString());
    }

    private Token ReadString()
    {
        Advance(); // Skip initial quote

        StringBuilder sb = new StringBuilder();
        while (Peek() != '"' && !IsAtEnd())
            sb.Append(Advance());

        Advance(); // Skip final quote

        return new Token(TokenType.String, sb.ToString());
    }

    private Token ReadNumber()
    {
        StringBuilder sb = new StringBuilder();
        while (char.IsDigit(Peek()))
            sb.Append(Advance());

        return new Token(TokenType.Number, sb.ToString());
    }

    private Token ReadIdentifier()
    {
        StringBuilder sb = new StringBuilder();
        while (char.IsLetter(Peek()))
            sb.Append(Advance());

        return new Token(TokenType.Identifier, sb.ToString());
    }

    private char Peek() => source[pos];
    private char Advance() => source[pos++];
    private bool IsAtEnd() => pos >= source.Length;
}
