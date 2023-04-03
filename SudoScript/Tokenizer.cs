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
public sealed class TokenStream
{

    private readonly TextReader _reader;

    private char? _carry;

    public TokenStream(TextReader reader) {
        _reader = reader;
        _carry = new();
        HasNext = true;
    }

    public TokenStream(string s) : this(new StringReader(s)){
    }

    private Token? Next;

    public bool HasNext { get; private set; }

    public bool Peek([NotNullWhen(true)] out Token? nextToken) {
        if(!HasNext) {
            nextToken = null;
            return false;
        } else if(Next != null) {
            nextToken = Next;
            return true;
        } else if(GetToken(out Token? result)) {
            nextToken = result;
            Next = result;
            return true;
        } else {
            nextToken = null;
            HasNext = false;
            return false;
        }
    }

    public bool Expect(TokenType type, [NotNullWhen(true)] out Token? token) {
        if(Next == null) {
            token = null;
            return false;
        }

        if(!Next.Type.Equals(type)) {
            throw new ArgumentException("The next type did not match with the expected type.");
        }

        if(Next == null) {
            return Peek(out token);
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

        List<char> match = new() { character };
        
        //read comment
        if(character == '/' && GetNextCharacter(out char secondCharacter)) {
            type = 0;
            List<char> matchList = new();

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
            //read as text
            char currentCharacter;
            while(GetNextCharacter(out currentCharacter) && char.IsLetterOrDigit(currentCharacter)) {
                match.Add(currentCharacter);
            }
            _carry = currentCharacter;

            type = new string(match.ToArray()) switch {
                "unit" =>  TokenType.Unit,
                "givens" => TokenType.Unit,
                "rules" => TokenType.Unit,
                _ => TokenType.Identifier,
            };
        } else if(Char.IsDigit(character)) {
            //read as digit
            char currentCharacter;
            while(GetNextCharacter(out currentCharacter) && Char.IsDigit(character)) {
                match.Add(currentCharacter);
            }
            _carry = currentCharacter;
            type = TokenType.Number;
        }else if(character == '\n') {
            //read newlines
            char currentCharacter;
            while(GetNextCharacter(out currentCharacter) && currentCharacter == '\n') {
                match.Add(currentCharacter);
            }
            _carry = currentCharacter;
            type = TokenType.Newline;
        } else if(char.IsWhiteSpace(character)){
            //read other whitespace
            char currentCharacter;
            while(GetNextCharacter(out currentCharacter) && char.IsWhiteSpace(currentCharacter)) {
                match.Add(currentCharacter);
            }
            _carry = currentCharacter;
            type = TokenType.Space;
        }

        if(type != 0) {
            nextToken = new Token(type, new string(match.ToArray()), "", 0, 0, "");
            return true;
        }

        throw new ArgumentException($"The next character: '{character}' was not recognized as a token!");
    }

    private bool GetNextCharacter(out char character) {
        if(_carry != null) {
            character = (char)_carry;
            _carry = null;
            return true;
        } else {
            int r = _reader.Read();
            character = (char)r;
            return r == -1;
        }
    }

    public void Dispose() {
        _reader.Dispose();
    }
}



//public bool Accept(TokenType type, [NotNullWhen(true)] out Token? token)
//{
//    throw new NotImplementedException();
//}

//public bool MoveNext()
//{

//    if(!GetNextCharacter(out char character)) {
//        return false;
//    }

//    /*

//    Types of tokens
//        single char tokens
//            
//        container tokens
//            LineComment,
//            BlockComment,           
//     */

//    return true; 
//}

//private void AssignToken(TokenType type, string match) {



//    //creates a new token given Type, Match,
//    //assigning it to current

//    throw new NotImplementedException();

//}

//public void Reset()
//{
//    throw new NotImplementedException();
//}

//public void Dispose()
//{
//    _reader.Dispose();
//}

//public Token Current => throw new NotImplementedException();

//object IEnumerator.Current => throw new NotImplementedException();