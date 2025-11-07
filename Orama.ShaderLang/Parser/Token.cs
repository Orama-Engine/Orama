using System;
using System.Collections.Generic;
using System.Text;

namespace Orama.ShaderLang.Parser;

internal enum TokenType
{
    Identifier,
    Number,
    String,
    LeftBrace,
    RightBrace,
    Hash,
    Equals,
    EndOfFile
}

internal class Token
{
    /// <summary> The type of the token. </summary>
    public TokenType Type { get; }

    /// <summary> The string value of the token. </summary>
    public string Value { get; }

    /// <summary> Initializes a new instance of the <see cref="Token"/> class. </summary>
    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Type}: {Value}";
}
