using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

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
        return new TokenStream(src);
    }

    public static TokenStream GetStream(StreamReader reader)
    {
        return new TokenStream(reader);
    }
}

/// Feel free to remove this comment afterwards, this is purely for implementation details.
/// <param name="Type">The type of the token</param>
/// <param name="Match">What this token matches with.</param>
/// <param name="Line">The line this token is found on. Try to re-use the same line for all tokens if at all possible.</param>
/// <param name="Row">The row of the start of this token.</param>
/// <param name="Column">The collumn of the start of this token.</param>
/// <param name="FileName">The file name of this token.</param>
public record Token(TokenType Type, string Match, string Line, int Row, int Column, string FileName);


// The token stream should read the tokens one at a time, so it doesn't use too much memory.
public sealed class TokenStream {

    private readonly TextReader _reader;

    private char? _carry;

    public TokenStream(TextReader reader) {
        _reader = reader;
        _carry = null;
        _hasNext = true;
    }

    public TokenStream(string s) : this(new StringReader(s)) {
    }

    private Token? Next;

    private bool _hasNext;
    public bool HasNext {
        get {
            Peek(out _);
            return _hasNext;
        }
        private set {
            _hasNext = value;
        }
    }

    public bool Peek([NotNullWhen(true)] out Token? nextToken) {
        if(!_hasNext) {
            nextToken = null;
            return false;
        } else if(Next is not null) {
            nextToken = Next;
            return true;
        } else if(GetToken(out Token? result)) {
            nextToken = result;
            Next = result;
            return true;
        } else {
            nextToken = null;
            _hasNext = false;
            return false;
        }
    }

    public bool Expect(TokenType type, [NotNullWhen(true)] out Token? token) {
        if(Next is null) {
            bool b = Peek(out token);
            Next = null;
            return b;
        } else if(!Next.Type.Equals(type)) {
            token = null;
            return false;
        } else {
            token = Next;
            Next = null;
            return true;
        }
    }

    private bool GetToken([NotNullWhen(true)] out Token? nextToken) {
        
        if(!GetNextCharacter(out char character)) {
            nextToken = null;
            return false;
        }

        //read any single digit character
        TokenType type = character switch {
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

        if(type != 0) {
            //TODO: Add line, row, column, filename.
            nextToken = new Token(type, character.ToString(), "", 0, 0, "");
            return true;
        }

        string match = "";

        if(character == '/' && GetNextCharacter(out char secondCharacter)) {
            List<char> matchList = new() { character, secondCharacter };

            if(secondCharacter == '/') {
                bool flag;
                do {
                    flag = GetNextCharacter(out char currentCharacter) && currentCharacter != '\n';
                    matchList.Add(currentCharacter);
                } while(flag);
                type = TokenType.LineComment;
            } else if(secondCharacter == '*') {
                char lastCharacter = default;

                bool flag;
                do {
                    flag = GetNextCharacter(out char currentCharacter) && lastCharacter != '*' && currentCharacter != '/';
                    matchList.Add(currentCharacter);
                    lastCharacter = currentCharacter;
                } while(flag);
                type = TokenType.BlockComment;
            }

        } else if(Char.IsLetter(character)) {
            match = character + MatchWhile(c => char.IsLetterOrDigit(c));
            type = match switch {
                "unit" =>  TokenType.Unit,
                "givens" => TokenType.Givens,
                "rules" => TokenType.Rules,
                _ => TokenType.Identifier,
            };
        } else if(Char.IsDigit(character)) {
            match = character + MatchWhile(c => Char.IsDigit(c));
            type = TokenType.Number;
        }else if(character == '\n') {
            match = character + MatchWhile(c => c == '\n');
            type = TokenType.Newline;
        } else if(char.IsWhiteSpace(character)){
            match = character + MatchWhile(c => char.IsWhiteSpace(c) && c != '\n');
            type = TokenType.Space;
        }

        if(type != 0) {
            nextToken = new Token(type, match, "", 0, 0, "");
            return true;
        }

        throw new ArgumentException($"Unrecognized character: {character} is not a recognised token");
    }

    private string MatchWhile(Func<char, bool> condition) {
        List<char> match = new();

        char currentCharacter;
        while(GetNextCharacter(out currentCharacter) && condition.Invoke(currentCharacter)) {
            match.Add(currentCharacter);
        }
        _carry = currentCharacter;

        return new string(match.ToArray());
    }

    private bool GetNextCharacter(out char character) {
        if(_carry is not null) {
            character = (char)_carry;
            _carry = null;
            return true;
        } else {
            int r = _reader.Read();
            character = (char)r;
            return r != -1;
        }
    }

    public void Dispose() {
        _reader.Dispose();
    }
}
