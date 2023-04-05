
using NUnit.Framework;
using SudoScript;

namespace Tests;

internal sealed class TokenizerTests {

    [Test]
    public void CanReadString() {

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

        for(int i = 0; i < expected.Length || tokenizer.HasNext; i++) {
            Assert.That(tokenizer.Expect(expected[i].type, out Token? token));
            Assert.That(token?.Match, Is.EqualTo(expected[i].match));
        }

    }

    [Test]
    public void IgnoresSpecialTokens() {

        string s = "unit /*This is a comment!*/{ (1, 7 + 5) }";

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

        for(int i = 0; i < expected.Length || tokenizer.HasNext; i++) {
            Assert.That(tokenizer.Expect(expected[i].type, out Token? token));
            Assert.That(token?.Match, Is.EqualTo(expected[i].match));
        }

    }

}