using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SudoScript;

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

public static class Tokenizer {
    public static TokenStream GetStream(string src) {
        return new TokenStream(src);
    }

    public static TokenStream GetStream(StreamReader reader) {
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

public sealed class TokenStream : IDisposable
{

    private readonly TextReader _reader;

    private readonly LinkedList<Token> _next;

    private char? _carry;

    private bool _endIsFound;

    public TokenStream(TextReader reader)
    {
        _reader = reader;
        _next = new();
        _carry = null;
        _endIsFound = false;

        Continue(false);
    }

    public TokenStream(string s) : this(new StringReader(s))
    {
    }

    public bool HasNext { get => _next.Any(); }

    public IEnumerable<Token> Next(bool IgnoreSpecial, Func<TokenType, bool>? stopCondition = null)
    {
        while(Peek(IgnoreSpecial, out Token ? next) && (stopCondition is null || stopCondition.Invoke(next.Type))) 
        { 
            yield return next;
            Continue(IgnoreSpecial);
        }
    }

    public bool Expect(TokenType expected, [NotNullWhen(true)]out Token? token)
    {
        bool ignoreSpecial = !IsSpecialToken(expected);
        bool b = Peek(ignoreSpecial, out token) && token.Type.Equals(expected);
        if(b) Continue(ignoreSpecial);
        return b;
    }

    private static bool IsSpecialToken(TokenType type)
    {
        return type is (TokenType.LineComment
                or TokenType.BlockComment
                or TokenType.Space
                or TokenType.Newline);
    }

    public bool Peek(bool IgnoreSpecial, [NotNullWhen(true)] out Token? token)
    {
        if(_next.First is null || _endIsFound)
        {
            token = null;
        }
        else if(IgnoreSpecial)
        {
            token = _next.Last();
        }
        else
        {
            token = _next.First();
        }

        return token is not null;
    }

    public void Continue(bool IgnoreSpecial)
    {
        if(_next.Any())
        {
            if(IgnoreSpecial)
            {
                _next.Clear();
            }
            else
            {
                _next.RemoveFirst();
            }
        }

        if(!_next.Any())
        {
            Resupply();
        }
    }


private void Resupply()
    {
        Token? token;
        do
        {
            if(GetToken(out token))
            {
                _next.AddLast(token);
            }
        } while(token is not null && IsSpecialToken(token.Type));
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
                bool flag;
                do
                {
                    flag = GetNextCharacter(out char currentCharacter) && currentCharacter != '\n';
                    matchList.Add(currentCharacter);
                } while(flag);
                type = TokenType.LineComment;
            }
            else if(secondCharacter == '*')
            {
                char lastCharacter = default;

                bool flag;
                do
                {
                    flag = GetNextCharacter(out char currentCharacter) && lastCharacter != '*' && currentCharacter != '/';
                    matchList.Add(currentCharacter);
                    lastCharacter = currentCharacter;
                } while(flag);
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
        else if(character == '\n')
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

        nextToken = new Token(type, match, "", 0, 0, "");
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

        char currentCharacter;
        while(GetNextCharacter(out currentCharacter) && condition.Invoke(currentCharacter))
        {
            match.Add(currentCharacter);
        }
        _carry = currentCharacter;

        return new string(match.ToArray());
    }

    public void Dispose()
    {
        _reader.Dispose();
    }

}

//public sealed class TokenStream {

//    private readonly TextReader _reader;

//    private char? _carry;

//    private readonly LinkedList<Token> _next;

//    private bool _hasNext;

//    public TokenStream(TextReader reader) {
//        _reader = reader;
//        _carry = null;
//        _hasNext = true;
//        _next = new();
//    }

//    public TokenStream(string s) : this(new StringReader(s)) {
//    }

//    public bool HasNext {
//        get {
//            _ = Peek(true, out _);
//            return _hasNext;
//        }
//        private set => _hasNext = value;
//    }

//    public bool Peek(bool ignoreSpecialTokens, [NotNullWhen(true)] out Token? nextToken) {
//        if(!_hasNext) {
//            nextToken = null;
//            return false;
//        } else if(_next.First is not null && _next.Last is not null) {
//            nextToken = ignoreSpecialTokens ? _next.Last.Value : _next.First.Value;
//            return true;
//        } else if(RetriveToken(ignoreSpecialTokens, out Token? result)) {
//            nextToken = result;
//            _ = _next.AddLast(result);
//            return true;
//        } else {
//            nextToken = null;
//            _hasNext = false;
//            return false;
//        }
//    }

//    public bool Expect(TokenType type, [NotNullWhen(true)] out Token? token) {
//        bool ignoreSpecial = !IsSpecialToken(type);

//        if(_next.First is not null && _next.Last is not null) {
//            token = ignoreSpecial ? _next.Last.Value : _next.First.Value;
//            if(token.Type.Equals(type)) {
//                if(ignoreSpecial) {
//                    _next.Clear();
//                } else {
//                    _next.RemoveFirst();
//                }
//                return true;
//            }
//            return false;
//        }

//        bool b = Peek(ignoreSpecial, out token) && token.Type.Equals(type);
//        _next.Clear();
//        return b;
//    }

//    private bool RetriveToken(bool ignoreSpecialToken, [NotNullWhen(true)] out Token? nextToken) {
//        bool flag;
//        do {
//            if(!GetToken(out nextToken)) {
//                return false;
//            }

//            flag = ignoreSpecialToken && IsSpecialToken(nextToken.Type);
//            if(flag) {
//                _ = _next.AddLast(nextToken);
//            }
//        } while(flag);

//        return true;
//    }

//    private static bool IsSpecialToken(TokenType type) {
//        return type is not (not TokenType.LineComment
//                and not TokenType.BlockComment
//                and not TokenType.Space
//                and not TokenType.Newline);
//    }

//    private bool GetToken([NotNullWhen(true)] out Token? nextToken) {

//        if(!GetNextCharacter(out char character)) {
//            nextToken = null;
//            return false;
//        }

//        TokenType type = character switch {
//            '{' => TokenType.LeftBrace,
//            '}' => TokenType.RightBrace,
//            '(' => TokenType.LeftParenthesis,
//            ')' => TokenType.RightParenthesis,
//            '?' => TokenType.QuestionMark,
//            '[' => TokenType.LeftBracket,
//            ']' => TokenType.RightBracket,
//            ',' => TokenType.Comma,
//            ';' => TokenType.Semicolon,
//            '+' => TokenType.Plus,
//            '-' => TokenType.Minus,
//            '*' => TokenType.Multiply,
//            '%' => TokenType.Mod,
//            '^' => TokenType.Power,
//            _ => 0,
//        };

//        string match;

//        if(type != 0) {
//            match = character.ToString();
//        } else if(character == '/' && GetNextCharacter(out char secondCharacter)) {
//            List<char> matchList = new() { character, secondCharacter };

//            if(secondCharacter == '/') {
//                bool flag;
//                do {
//                    flag = GetNextCharacter(out char currentCharacter) && currentCharacter != '\n';
//                    matchList.Add(currentCharacter);
//                } while(flag);
//                type = TokenType.LineComment;
//            } else if(secondCharacter == '*') {
//                char lastCharacter = default;

//                bool flag;
//                do {
//                    flag = GetNextCharacter(out char currentCharacter) && lastCharacter != '*' && currentCharacter != '/';
//                    matchList.Add(currentCharacter);
//                    lastCharacter = currentCharacter;
//                } while(flag);
//                type = TokenType.BlockComment;
//            }
//            match = new string(matchList.ToArray());
//        } else if(char.IsLetter(character)) {
//            match = character + MatchWhile(c => char.IsLetterOrDigit(c));
//            type = match switch {
//                "unit" => TokenType.Unit,
//                "givens" => TokenType.Givens,
//                "rules" => TokenType.Rules,
//                _ => TokenType.Identifier,
//            };
//        } else if(char.IsDigit(character)) {
//            match = character + MatchWhile(c => char.IsDigit(c));
//            type = TokenType.Number;
//        } else if(character == '\n') {
//            match = character + MatchWhile(c => c == '\n');
//            type = TokenType.Newline;
//        } else if(char.IsWhiteSpace(character)) {
//            match = character + MatchWhile(c => char.IsWhiteSpace(c) && c != '\n');
//            type = TokenType.Space;
//        } else {
//            throw new ArgumentException($"Unrecognized character: {character} is not a recognised token");
//        }

//        nextToken = new Token(type, match, "", 0, 0, "");
//        return true;
//    }

//    private string MatchWhile(Func<char, bool> condition) {
//        List<char> match = new();

//        char currentCharacter;
//        while(GetNextCharacter(out currentCharacter) && condition.Invoke(currentCharacter)) {
//            match.Add(currentCharacter);
//        }
//        _carry = currentCharacter;

//        return new string(match.ToArray());
//    }

//    private bool GetNextCharacter(out char character) {
//        if(_carry is not null) {
//            character = (char)_carry;
//            _carry = null;
//            return true;
//        } else {
//            int r = _reader.Read();
//            character = (char)r;
//            return r != -1;
//        }
//    }

//    public void Dispose() {
//        _reader.Dispose();
//    }
//}
