
using NUnit.Framework;
using SudoScript.Core;

namespace SudoScript.Core.Test;

internal sealed class TokenizerTests
{

    [Test]
    public void CanReadString()
    {
        string s = "unit { (1, 7 + 5) }";

        TokenStream tokenizer = new(s);

        (TokenType type, string match)[] expected = new (TokenType type, string match)[] {
            (TokenType.Unit, "unit"),
            (TokenType.Space, " "),
            (TokenType.LeftBrace, "{"),
            (TokenType.Space, " "),
            (TokenType.LeftParenthesis, "("),
            (TokenType.Number, "1"),
            (TokenType.Comma, ","),
            (TokenType.Space, " "),
            (TokenType.Number, "7"),
            (TokenType.Space, " "),
            (TokenType.Plus, "+"),
            (TokenType.Space, " "),
            (TokenType.Number, "5"),
            (TokenType.RightParenthesis, ")"),
            (TokenType.Space, " "),
            (TokenType.RightBrace, "}"),
        };

        for(int i = 0; i < expected.Length || tokenizer.HasNext; i++)
        {
            Assert.That(tokenizer.Expect(expected[i].type, out Token? token));
            Assert.That(token?.Match, Is.EqualTo(expected[i].match));
        }


        Assert.That(tokenizer.HasNext, Is.False);
        Assert.That(tokenizer.HasSpecialNext, Is.False);
        Assert.That(tokenizer.HasNonSpecialNext, Is.False);
    }

    [Test]
    public void IgnoresSpecialTokens()
    {
        string s = "unit /*This is a comment!*/{ (1, 7 + 5) } ";

        TokenStream tokenizer = new(s);

        (TokenType type, string match)[] expected = new (TokenType type, string match)[] {
            (TokenType.Unit, "unit"),
            (TokenType.LeftBrace, "{"),
            (TokenType.LeftParenthesis, "("),
            (TokenType.Number, "1"),
            (TokenType.Comma, ","),
            (TokenType.Number, "7"),
            (TokenType.Space, " "),
            (TokenType.Plus, "+"),
            (TokenType.Number, "5"),
            (TokenType.RightParenthesis, ")"),
            (TokenType.RightBrace, "}"),
        };

        for(int i = 0; i < expected.Length; i++)
        {
            Assert.That(tokenizer.Expect(expected[i].type, out Token? token));
            Assert.That(token?.Match, Is.EqualTo(expected[i].match));
        }

        Assert.That(tokenizer.HasNext, Is.True);
        Assert.That(tokenizer.HasSpecialNext, Is.True);
        Assert.That(tokenizer.HasNonSpecialNext, Is.False);
    }

    // Line Information tests

    [Test]
    public void LineInformationTest()
    {
        string s = 
@"unit {
    (1,5)
}";

        TokenStream tokenizer = new(s);

        (TokenType type, string match, int row, int column)[] expected = new (TokenType type, string match, int row, int column)[] {
            (TokenType.Unit, "unit", 1, 1),
            (TokenType.LeftBrace, "{", 1, 6),
            (TokenType.Newline, "\n", 1, 7),
            (TokenType.LeftParenthesis, "(", 2, 5),
            (TokenType.Number, "1", 2, 6),
            (TokenType.Comma, ",", 2, 7),
            (TokenType.Number, "5", 2, 8),
            (TokenType.RightParenthesis, ")", 2, 9),
            (TokenType.Newline, "\n", 2, 10),
            (TokenType.RightBrace, "}", 3, 1),
        };

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.That(tokenizer.Expect(expected[i].type, out Token? token));
            Assert.That(token?.Row, Is.EqualTo(expected[i].row));
            Assert.That(token?.Column, Is.EqualTo(expected[i].column));
        }
    }

    [Test]
    public void LineInformationWithLineCommentNewLineTest()
    {
        string s =
@"unit {
    //this is a line comment
    (1,5)
}";

        TokenStream tokenizer = new(s);

        (TokenType type, string match, int row, int column)[] expected = new (TokenType type, string match, int row, int column)[] {
            (TokenType.Unit, "unit", 1, 1),
            (TokenType.LeftBrace, "{", 1, 6),
            (TokenType.Newline, "\n", 1, 7),
            (TokenType.LeftParenthesis, "(", 3, 5),
            (TokenType.Number, "1", 3, 6),
            (TokenType.Comma, ",", 3, 7),
            (TokenType.Number, "5", 3, 8),
            (TokenType.RightParenthesis, ")", 3, 9),
            (TokenType.Newline, "\n", 3, 10),
            (TokenType.RightBrace, "}", 4, 1),
        };

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.That(tokenizer.Expect(expected[i].type, out Token? token));
            Assert.That(token?.Row, Is.EqualTo(expected[i].row));
            Assert.That(token?.Column, Is.EqualTo(expected[i].column));
        }
    }

    [Test]
    public void LineInformationWithLineCommentSameLineTest()
    {
        string s =
@"unit {
    (1,5)//this is a line comment
}";

        TokenStream tokenizer = new(s);

        (TokenType type, string match, int row, int column)[] expected = new (TokenType type, string match, int row, int column)[] {
            (TokenType.Unit, "unit", 1, 1),
            (TokenType.LeftBrace, "{", 1, 6),
            (TokenType.Newline, "\n", 1, 7),
            (TokenType.LeftParenthesis, "(", 2, 5),
            (TokenType.Number, "1", 2, 6),
            (TokenType.Comma, ",", 2, 7),
            (TokenType.Number, "5", 2, 8),
            (TokenType.RightParenthesis, ")", 2, 9),
            (TokenType.RightBrace, "}", 3, 1),
        };

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.That(tokenizer.Expect(expected[i].type, out Token? token));
            Assert.That(token?.Row, Is.EqualTo(expected[i].row));
            Assert.That(token?.Column, Is.EqualTo(expected[i].column));
        }
    }

    [Test]
    public void LineInformationWithBlockCommentTest()
    {
        string s =
@"unit {
    /*
    this is a block comment
    */
    (1,5)
}";

        TokenStream tokenizer = new(s);

        (TokenType type, string match, int row, int column)[] expected = new (TokenType type, string match, int row, int column)[] {
            (TokenType.Unit, "unit", 1, 1),
            (TokenType.LeftBrace, "{", 1, 6),
            (TokenType.Newline, "\n", 1, 7),
            (TokenType.LeftParenthesis, "(", 5, 5),
            (TokenType.Number, "1", 5, 6),
            (TokenType.Comma, ",", 5, 7),
            (TokenType.Number, "5", 5, 8),
            (TokenType.RightParenthesis, ")", 5, 9),
            (TokenType.Newline, "\n", 5, 10),
            (TokenType.RightBrace, "}", 6, 1),
        };

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.That(tokenizer.Expect(expected[i].type, out Token? token));
            Assert.That(token?.Row, Is.EqualTo(expected[i].row));
            Assert.That(token?.Column, Is.EqualTo(expected[i].column));
        }
    }

    [Test]
    public void LineInformationWithBlockCommentPreLineTest()
    {
        string s =
@"unit {
    /*this is a block comment*/(1,5)
}";

        TokenStream tokenizer = new(s);

        (TokenType type, string match, int row, int column)[] expected = new (TokenType type, string match, int row, int column)[] {
            (TokenType.Unit, "unit", 1, 1),
            (TokenType.LeftBrace, "{", 1, 6),
            (TokenType.Newline, "\n", 1, 7),
            (TokenType.LeftParenthesis, "(", 2, 32),
            (TokenType.Number, "1", 2, 33),
            (TokenType.Comma, ",", 2, 34),
            (TokenType.Number, "5", 2, 35),
            (TokenType.RightParenthesis, ")", 2, 36),
            (TokenType.Newline, "\n", 2, 37),
            (TokenType.RightBrace, "}", 3, 1),
        };

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.That(tokenizer.Expect(expected[i].type, out Token? token));
            Assert.That(token?.Row, Is.EqualTo(expected[i].row));
            Assert.That(token?.Column, Is.EqualTo(expected[i].column));
        }
    }

    [Test]
    public void LineInformationWithBlockCommentEndOnLineTest()
    {
        string s =
@"unit {
    /*this is a 
    block comment*/(1,5)
}";

        TokenStream tokenizer = new(s);

        (TokenType type, string match, int row, int column)[] expected = new (TokenType type, string match, int row, int column)[] {
            (TokenType.Unit, "unit", 1, 1),
            (TokenType.LeftBrace, "{", 1, 6),
            (TokenType.Newline, "\n", 1, 7),
            (TokenType.LeftParenthesis, "(", 3, 20),
            (TokenType.Number, "1", 3, 21),
            (TokenType.Comma, ",", 3, 22),
            (TokenType.Number, "5", 3, 23),
            (TokenType.RightParenthesis, ")", 3, 24),
            (TokenType.Newline, "\n", 3, 25),
            (TokenType.RightBrace, "}", 4, 1),
        };

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.That(tokenizer.Expect(expected[i].type, out Token? token));
            Assert.That(token?.Row, Is.EqualTo(expected[i].row));
            Assert.That(token?.Column, Is.EqualTo(expected[i].column));
        }
    }

    [Test]
    public void LineInformationWithBlockCommentEndOfFileTest()
    {
        string s =
@"unit {
    (1,5)
}/*this is a block comment*/";

        TokenStream tokenizer = new(s);

        (TokenType type, string match, int row, int column)[] expected = new (TokenType type, string match, int row, int column)[] {
            (TokenType.Unit, "unit", 1, 1),
            (TokenType.LeftBrace, "{", 1, 6),
            (TokenType.Newline, "\n", 1, 7),
            (TokenType.LeftParenthesis, "(", 2, 5),
            (TokenType.Number, "1", 2, 6),
            (TokenType.Comma, ",", 2, 7),
            (TokenType.Number, "5", 2, 8),
            (TokenType.RightParenthesis, ")", 2, 9),
            (TokenType.Newline, "\n", 2, 10),
            (TokenType.RightBrace, "}", 3, 1),
        };

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.That(tokenizer.Expect(expected[i].type, out Token? token));
            Assert.That(token?.Row, Is.EqualTo(expected[i].row));
            Assert.That(token?.Column, Is.EqualTo(expected[i].column));
        }
    }
}