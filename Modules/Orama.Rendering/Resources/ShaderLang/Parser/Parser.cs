
using System.Reflection;

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
                ParseIdentifierOrBlock(shader);
            }
            else if (token.Type == TokenType.Hash)
            {
                ParseMetaData(shader);
                continue;
            }
        }

        return shader;
    }

    private void ParseIdentifierOrBlock(ShaderLangFormat shader)
    {
        string key = Advance().Value;

        // key = "value"
        if (Check(TokenType.Equals))
        {
            Advance();
            if (Check(TokenType.String) || Check(TokenType.Identifier))
            {
                string value = Advance().Value;

                var prop = typeof(ShaderLangFormat).GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (prop != null && prop.CanWrite && prop.PropertyType == typeof(string))
                    prop.SetValue(shader, value);
                else
                    shader.MetaData[key] = value;
            }
        }
        // Block {  }
        else if (Check(TokenType.Block))
        {
            string body = Advance().Value;

            var prop = typeof(ShaderLangFormat)
                .GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (prop != null)
            {
                object? instance = prop.GetValue(shader);
                if (instance == null)
                {
                    instance = Activator.CreateInstance(prop.PropertyType);
                    prop.SetValue(shader, instance);
                }

                var bodyProp = prop.PropertyType.GetProperty("Body", BindingFlags.Public | BindingFlags.Instance);
                if (bodyProp != null && bodyProp.CanWrite && bodyProp.PropertyType == typeof(string))
                    bodyProp.SetValue(instance, body);
            }
        }
    }


    private void ParseMetaData(ShaderLangFormat shader)
    {
        Advance();

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
