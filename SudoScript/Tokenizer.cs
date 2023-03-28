using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SudoScript;

public enum TokenType
{
    LineComment,
    BlockComment,
    Unit,
    Givens,
    Rules,
    Identifier,
    Number,
    LeftBrace,
    RightBrace,
    LeftParenthesis,
    RightParenthesis,
    QuestionMark,
    LeftBracket,
    RightBracket,
    Comma,
    Semicolon,
    Space,
    Newline,
    Plus,
    Minus,
    Multiply,
    Mod,
    Power,
}

public static class Tokenizer
{
    public static TokenStream GetStream(string src)
    {
        throw new NotImplementedException();
        // Return the tokens like this.
        // yield return new Token();
    }

    public static TokenStream GetStream(StreamReader reader)
    {
        throw new NotImplementedException();
        // Return the tokens like this.
        // yield return new Token();
    }
}

/// Feel free to remove this comment afterwards, this is purely for implementation details.
/// <param name="Type">The type of the token</param>
/// <param name="Match">What this token matches with.</param>
/// <param name="Line">The line this token is found on. Try to re-use the same line for all tokens if at all possible.</param>
/// <param name="Row">The row of the start of this token.</param>
/// <param name="Collumn">The collumn of the start of this token.</param>
/// <param name="FileName">The file name of this token.</param>
public record Token(TokenType Type, string Match, string Line, int Row, int Collumn, string FileName);


// The token stream should read the tokens one at a time, so it doesn't use too much memory.
public sealed class TokenStream : IEnumerator<Token>
{
    public bool Accept(TokenType type, [NotNullWhen(true)] out Token? token)
    {
        throw new NotImplementedException();
    }

    public bool MoveNext()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Token Current => throw new NotImplementedException();

    object IEnumerator.Current => throw new NotImplementedException();
}