using System.Diagnostics.CodeAnalysis;

namespace SudoScript.Core;

public enum TokenType {
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

/// Feel free to remove this comment afterwards, this is purely for implementation details.
/// <param name="Type">The type of the token</param>
/// <param name="Match">What this token matches with.</param>
/// <param name="Row">The row of the start of this token.</param>
/// <param name="Column">The collumn of the start of this token.</param>
/// <param name="FileName">The file name of this token.</param>
public record Token(TokenType Type, string Match, int Row, int Column, string FileName);

public sealed class TokenStream : IDisposable
{
    int row = 1;
    int column = 1;

    private readonly TextReader _reader;

    private readonly List<Token> _next; //TODO: Check if linkedlist or queue is faster than queue.

    private char? _carry;

    public TokenStream(TextReader reader)
    {
        _reader = reader;
        _next = new();
        _carry = null;

        Continue(false);
    }

    public TokenStream(string s) : this(new StringReader(s))
    {
    }

    public bool HasNext { get => _next.Any(); }
    public bool HasNonSpecialNext { get => ((IEnumerable<Token>)_next).Reverse().Any(t => !IsSpecialToken(t.Type)); }
    public bool HasSpecialNext { get => _next.Any(t => IsSpecialToken(t.Type)); }

    public IEnumerable<Token> Next(bool ignoreSpecial, Func<TokenType, bool>? stopCondition = null)
    {
        while(Peek(ignoreSpecial, out Token ? next) && (stopCondition is null || stopCondition.Invoke(next.Type))) 
        { 
            yield return next;
            Continue(ignoreSpecial);
        }
    }

    public bool Expect(TokenType expected, [NotNullWhen(true)]out Token? token)
    {
        bool ignoreSpecial = !IsSpecialToken(expected);
       
        if(Peek(ignoreSpecial, out token) && token.Type.Equals(expected))
        {
            Continue(ignoreSpecial);
            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool IsSpecialToken(TokenType type)
    {
        return type is (TokenType.LineComment
                or TokenType.BlockComment
                or TokenType.Space
                or TokenType.Newline);
    }

    public bool Peek(bool ignoreSpecial, [NotNullWhen(true)] out Token? token)
    {
        if(_next is null || !_next.Any())
        {
            token = null;
        }
        else if(ignoreSpecial)
        {
            token = _next.Last();
        }
        else
        {
            token = _next.First();
        }

        return token is not null;
    }

    public void Continue(bool ignoreSpecial)
    {
        // Next contains zero or more special tokens and might contain one non special
        // token in the end.
        // To continue to the next token, we remove either the next token, or the next
        // non special token (including all special tokens that presceed it.)

        if(ignoreSpecial)
        {
            _next.Clear();
        }
        else if(_next.Any())
        {
            _next.RemoveAt(0);
        }

        if(!_next.Any())
        {
            Token? token;
            do
            {
                if(GetToken(out token))
                {
                    _next.Add(token);
                }
            } while(token is not null && IsSpecialToken(token.Type));
        }
    }

    private bool GetToken([NotNullWhen(true)] out Token? nextToken)
    {

        if(!GetNextCharacter(out char character))
        {
            nextToken = null;
            return false;
        }

        TokenType type = character switch
        {
            '{' => TokenType.LeftBrace,
            '}' => TokenType.RightBrace,
            '(' => TokenType.LeftParenthesis,
            ')' => TokenType.RightParenthesis,
            '?' => TokenType.QuestionMark,
            '[' => TokenType.LeftBracket,
            ']' => TokenType.RightBracket,
            ',' => TokenType.Comma,
            ';' => TokenType.Semicolon,
            '+' => TokenType.Plus,
            '-' => TokenType.Minus,
            '*' => TokenType.Multiply,
            '%' => TokenType.Mod,
            '^' => TokenType.Power,
            _ => 0,
        };

        string match;

        if(type != 0)
        {
            match = character.ToString();
        }
        else if(character == '/' && GetNextCharacter(out char secondCharacter))
        {
            List<char> matchList = new() { character, secondCharacter };

            if(secondCharacter == '/')
            {
                bool flag = true;
                while(flag)
                {
                    flag = GetNextCharacter(out char currentCharacter) && currentCharacter != '\n';
                    matchList.Add(currentCharacter);
                }
                type = TokenType.LineComment;
            }
            else if(secondCharacter == '*')
            {
                char lastCharacter = default;
                char currentCharacter = '\0';

                bool flag = true;
                while(flag)
                {
                    if(GetNextCharacter(out currentCharacter))
                    {
                        if(lastCharacter != '*' && currentCharacter != '/')
                        {
                            matchList.Add(currentCharacter);
                            flag = true;
                            lastCharacter = currentCharacter;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Unrecognized character: {character} is not a recognised token");
                    }

                }
                matchList.Add(currentCharacter);
                type = TokenType.BlockComment;
            }
            match = new string(matchList.ToArray());
        }
        else if(char.IsLetter(character))
        {
            match = character + MatchWhile(c => char.IsLetterOrDigit(c));
            type = match switch
            {
                "unit" => TokenType.Unit,
                "givens" => TokenType.Givens,
                "rules" => TokenType.Rules,
                _ => TokenType.Identifier,
            };
        }
        else if(char.IsDigit(character))
        {
            match = character + MatchWhile(c => char.IsDigit(c));
            type = TokenType.Number;
        }
        else if(character == '\n' || character == '\r' && GetNextCharacter(out char currentCharacter) && currentCharacter == '\n')
        {
            match = character + MatchWhile(c => c == '\n');
            type = TokenType.Newline;
        }
        else if(char.IsWhiteSpace(character))
        {
            match = character + MatchWhile(c => char.IsWhiteSpace(c) && c != '\n');
            type = TokenType.Space;
        }
        else
        {
            throw new ArgumentException($"Unrecognized character: {character} is not a recognised token");
        }

        nextToken = new Token(type, match, row, column, "");

        if (nextToken.Type == TokenType.BlockComment || nextToken.Type == TokenType.LineComment)
        {
            for (int i = 0; i < nextToken.Match.Length; i++)
            {
                column++;
                if (nextToken.Match[i] == '\n')
                {
                    row++;
                    column = 1;
                }
            }
        }
        else if (nextToken.Type == TokenType.Newline)
        {
            row++;
            column = 1;
        }
        else
        {
            column += nextToken.Match.Length;
        }

        return true;
    }

    private bool GetNextCharacter(out char character)
    {
        if(_carry is not null)
        {
            character = (char)_carry;
            _carry = null;
            return true;
        }
        else
        {
            int r = _reader.Read();
            character = (char)r;
            return r != -1;
        }
    }

    private string MatchWhile(Func<char, bool> condition)
    {
        List<char> match = new();

        bool flag = true;
        while(flag)
        {
            if(GetNextCharacter(out char currentCharacter))
            {
                if(condition.Invoke(currentCharacter))
                {
                    flag = true;
                    match.Add(currentCharacter);
                }
                else
                {
                    _carry = currentCharacter;
                    flag = false;
                }
            }
            else
            {
                _carry = null;
                flag = false;
            }
        }
        
        return new string(match.ToArray());
    }

    public void Dispose()
    {
        _reader.Dispose();
    }

}
