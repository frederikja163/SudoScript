
using SudoScript.Core.Ast;
using System.Linq;

namespace SudoScript.Core;

public static class ExpressionParser
{

    public static ArgumentNode ParseElement(TokenStream stream)
    {
        if(!stream.Peek(true, out Token? peekedToken))
        {
            throw new ArgumentException();
        }

        ArgumentNode argument = null;
        switch(peekedToken.Type)
        {
            case TokenType.LeftBracket or TokenType.RightBracket : 
                argument = ParseRange(stream); 
                break;
            case TokenType.LeftParenthesis : 
                argument = ParseCellOrParenthesis(stream); 
                break;
            case TokenType.Identifier :
                stream.Continue(true);
                argument = new IdentifierNode(peekedToken); 
                break;
            case TokenType.Number :
                stream.Continue(true);
                argument = new ValueNode(peekedToken); 
                break;
            default: 
                throw new ArgumentException(); 
        };
        return argument;
    }

    private static ExpressionNode ParseRange(TokenStream stream)
    {
        if(!stream.Peek(true, out Token? peekedMinToken))
        {
            throw new ArgumentException();
        }

        stream.Continue(true);

        bool isMinInclusive = peekedMinToken.Type == TokenType.LeftBracket;
        if(!isMinInclusive && peekedMinToken.Type != TokenType.RightBrace)
        {
            throw new ArgumentException("Expected bracket for range!");
        }

        // Parses x
        ExpressionNode x = Parse(stream);

        // Since x and y are ; separated, parses comma.
        if(!stream.Expect(TokenType.Semicolon, out Token? semicolonToken))
        {
            throw new Exception(", expected");
        }

        // Parses y.
        ExpressionNode y = Parse(stream);

        if(!stream.Peek(true, out Token? peekedMaxToken))
        {
            throw new ArgumentException();
        }

        stream.Continue(true);

        bool isMaxInclusive = peekedMaxToken.Type == TokenType.LeftBracket;
        if(!isMaxInclusive && peekedMaxToken.Type != TokenType.RightBrace)
        {
            throw new ArgumentException("Expected bracket for range!");
        }

        return new RangeNode(peekedMinToken, peekedMaxToken, semicolonToken, x, y, isMinInclusive, isMaxInclusive);
    }

    private static ArgumentNode ParseCellOrParenthesis(TokenStream stream)
    {
        if(!stream.Expect(TokenType.LeftParenthesis, out Token? startToken))
        {
            throw new NotImplementedException();
        }

        // Parses x (or maybe the only expression)
        ExpressionNode x = Parse(stream);

        // Since x and y are comma separated, parses comma.
        if(!stream.Peek(true, out Token? peekedToken))
        {
            throw new ArgumentException("File-ended abruptly, expected comma or right-parenthesis!");
        }

        if(peekedToken.Type == TokenType.Comma)
        {
            stream.Continue(true);

            // Parses y.
            ExpressionNode y = Parse(stream);

            // Parses RightParenthesis.
            if(!stream.Expect(TokenType.RightParenthesis, out Token? endToken))
            {
                throw new Exception(") expected");
            }

            return new CellNode(startToken, endToken, peekedToken, x, y);
        }
        else if(peekedToken.Type == TokenType.RightParenthesis)
        {
            return x;
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public static ExpressionNode Parse(TokenStream stream)
    {
        ArgumentNode node = ParseElement(stream);

        if(node is ExpressionNode expression)
        {
            return expression;
        }
        else
        {
            throw new ArgumentException("Expected expression, received cell!");
        }
    }
}