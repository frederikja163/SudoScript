
using SudoScript.Core.Ast;
namespace SudoScript.Core;

public static class ExpressionParser
{

    public static ArgumentNode ParseElement(TokenStream stream)
    {
        if(!stream.Peek(true, out Token? peekedToken))
        {
            throw new ArgumentException();
        }

        ArgumentNode argument;
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

    private static ExpressionNode ParseElementAsExpression(TokenStream stream)
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
        List<object> expressionStack = new();

        Token? peekedToken = null;
        Token? previousOperator = null;

        int layer = 0;

        bool loopFlag = true;
        bool doPeekFlag = true;

        while(loopFlag)
        {
            if(doPeekFlag)
            {
                stream.Next(false, t => t == TokenType.Space || t == TokenType.BlockComment || t == TokenType.LineComment);
                if(stream.Peek(false, out peekedToken) && 
                    (peekedToken.Type == TokenType.Space || 
                    peekedToken.Type == TokenType.BlockComment || 
                    peekedToken.Type == TokenType.LineComment))
                {
                    stream.Continue(false);
                }

                loopFlag = stream.Peek(false, out peekedToken) &&
                    (peekedToken is null ||
                    (peekedToken.Type != TokenType.RightParenthesis || layer != 0) &&
                    peekedToken.Type != TokenType.Newline &&
                    peekedToken.Type != TokenType.Comma);
            }

            int previousPrecedence = previousOperator is not null ? OperatorPrecedence(previousOperator.Type) : int.MaxValue ;
            int currentPrecedence = peekedToken is not null ? OperatorPrecedence(peekedToken.Type) : int.MaxValue;

            if(expressionStack.Count > 0 &&
                (previousPrecedence < currentPrecedence ||
                previousPrecedence == currentPrecedence &&
                peekedToken is not null && OperatorAssociativityIsLeftRight(peekedToken.Type)))
            {
                previousOperator = Reduce(expressionStack);
                doPeekFlag = false;
                loopFlag = true;
                continue;
            }
            else
            {
                if(doPeekFlag && loopFlag && peekedToken is not null)
                {
                    expressionStack.Add(peekedToken);
                    stream.Continue(false);
                    previousOperator = peekedToken;
                    peekedToken = null;
                }
                doPeekFlag = true;
            }
        }

        if(expressionStack.Count == 1 && expressionStack[^1] is ExpressionNode expression)
        {
            return expression;
        }
        else
        {
            throw new ArgumentException("An expression was incorrectly formatted!");
        }
    }
    
    private static Token? Reduce(List<object> expressionStack)
    {
        if(expressionStack.Count >= 3 &&
                expressionStack[^3] is ExpressionNode leftOperand &&
                expressionStack[^2] is Token binaryOperatorToken &&
                expressionStack[^1] is ExpressionNode rightOperand)
        { //binary operators
            expressionStack.RemoveRange(expressionStack.Count - 3, 3);
            BinaryType type = binaryOperatorToken.Type switch
            {
                TokenType.Plus => BinaryType.Plus,
                TokenType.Minus => BinaryType.Minus,
                TokenType.Multiply => BinaryType.Multiply,
                TokenType.Mod => BinaryType.Mod,
                TokenType.Power => BinaryType.Power,
                _ => throw new ArgumentException(),
            };
            expressionStack.Add(new BinaryNode(binaryOperatorToken, type, leftOperand, rightOperand));
        }
        else if(expressionStack.Count >= 2 &&
                expressionStack[^2] is Token unaryOperatorToken &&
                expressionStack[^1] is ExpressionNode operand)
        {
            expressionStack.RemoveRange(expressionStack.Count - 4, 3);
            UnaryType type = unaryOperatorToken.Type switch
            {
                TokenType.Plus => UnaryType.Plus,
                TokenType.Minus => UnaryType.Minus,
                _ => throw new ArgumentException(),
            };
            expressionStack.Add(new UnaryNode(unaryOperatorToken, type, operand));
        }else if(expressionStack.Count >= 1 &&
            expressionStack[^1] is Token elementToken)
        {
            expressionStack[^1] = elementToken.Type switch
            {
                TokenType.Identifier => new IdentifierNode(elementToken),
                TokenType.Number => new ValueNode(elementToken),
                _ => throw new ArgumentException(),
            };
        }
        else
        {

        }

        //find new previous operator
        for(int i = expressionStack.Count - 1; i >= 0; i--)
        {
            if(expressionStack[i] is Token operatorToken)
            {
                return operatorToken;
            }
        }

        return null;
    }

    //TODO: Consider inverting precedence order.
    private static int OperatorPrecedence(TokenType type) => type switch
    {
        TokenType.LeftParenthesis => 5,
        TokenType.Plus => 3,
        TokenType.Minus => 3,
        TokenType.Multiply => 2,
        TokenType.Mod => 2,
        TokenType.Power => 1,
        TokenType.RightParenthesis => 0,
        TokenType.Identifier => -1,
        TokenType.Number => -1,
        _ => int.MaxValue
    };

    //negation should have different associativity

    private static bool OperatorAssociativityIsLeftRight(TokenType type) => type switch
    {
        TokenType.Plus              => true,
        TokenType.Minus             => true,
        TokenType.Multiply          => true,
        TokenType.Mod               => true,
        TokenType.Power             => false,
        TokenType.LeftParenthesis   => false,
        TokenType.RightParenthesis  => true,
        TokenType.Identifier => true,
        TokenType.Number => true,
        _ => false
    };
}