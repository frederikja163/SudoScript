using SudoScript.Ast;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace SudoScript;

public static class Parser
{
    // Consider implementing this in a style similar to: https://github.com/frederikja163/TuringComplete
    public static ProgramNode ParseProgram(string src) => ParseProgram(Tokenizer.GetStream(src));

    public static ProgramNode ParseProgram(StreamReader reader) => ParseProgram(Tokenizer.GetStream(reader));

    private static ProgramNode ParseProgram(TokenStream stream)
    {
        return new ProgramNode(new UnitNode(null, ParseUnitStatements(stream), new()));
    }
    private static List<UnitStatementNode> ParseUnitStatements(TokenStream stream)
    {
        List<UnitStatementNode> children = new List<UnitStatementNode>();

        if (stream.HasNext && stream.Peek(true, out Token? rightBrace) && rightBrace.Type != TokenType.RightBrace)
        {
            children.Add(ParseUnitStatement(stream));
            if (!stream.HasNext || (stream.Peek(true, out rightBrace) && rightBrace.Type == TokenType.RightBrace))
                return children;

            stream.Expect(TokenType.Newline, out Token? _);
            children.AddRange(ParseUnitStatements(stream));
        }

        return children;
    }

    private static UnitStatementNode ParseUnitStatement(TokenStream stream)
    {
        if (stream.HasNext)
        {
            stream.Peek(true, out Token? token);
            switch (token.Type)
            {
                case TokenType.Unit:
                    return ParseUnit(stream);
                case TokenType.Identifier:
                    return ParseFunctionCall(stream);
                case TokenType.Rules:
                    return ParseRules(stream);
                case TokenType.Givens:
                    return ParseGivens(stream);
                default:
                    throw new Exception("syntax error");
            }
        }
        return new UnitNode(null, new(), new());
    }

    private static UnitNode ParseUnit(TokenStream stream)
    {
        List<ParameterNode> paramChildren = new List<ParameterNode>();

        stream.Expect(TokenType.Unit, out Token? unit);

        if (stream.Expect(TokenType.Identifier, out Token? identifier))
        {
            if (stream.Peek(true, out Token? paramId) && paramId.Type == TokenType.Identifier)
                paramChildren.AddRange(ParseParameters(stream));
        }

        if (!stream.Expect(TokenType.LeftBrace, out Token? test))
            throw new Exception("{ expected");

        if (stream.Peek(true, out Token? id) && id.Type == TokenType.RightBrace)
            return new UnitNode(identifier, new(), paramChildren);

        UnitNode unitNode = new UnitNode(identifier, ParseUnitStatements(stream), paramChildren);

        if (stream.Expect(TokenType.RightBrace, out Token? rightBrace))
        {
            return unitNode;
        }

        throw new Exception("} expected");
    }

    private static List<ParameterNode> ParseParameters(TokenStream stream)
    {
        List<ParameterNode> paramChildren = new List<ParameterNode>();
        // Check if it is a identifier or cell param
        while (!stream.Expect(TokenType.LeftBrace, out Token? _))
        {
            if(!stream.Expect(TokenType.Space, out Token _))
                throw new Exception("Params are expected to be separated by space");

            if (stream.Peek(true, out Token? leftParenthesis) && leftParenthesis.Type == TokenType.LeftParenthesis)
                paramChildren.Add(ParseParameterCell(stream));

            if (stream.Peek(true, out Token? identifier) && identifier.Type == TokenType.Identifier)
                paramChildren.Add(ParseParameterIdentifier(stream));

        }

        return paramChildren;
    }

    private static ParameterIdentifierNode ParseParameterIdentifier(TokenStream stream)
    {
        if (!stream.Expect(TokenType.Identifier, out Token? identifier))
            throw new Exception("Expected identifier");

        return new ParameterIdentifierNode(identifier);
    }

    private static ParameterCellNode ParseParameterCell(TokenStream stream)
    {
        ParameterIdentifierNode x;
        ParameterIdentifierNode y;

        stream.Expect(TokenType.LeftParenthesis, out Token? _);

        if (stream.Peek(true, out Token? identifier) && identifier.Type != TokenType.Identifier)
            throw new Exception("Expected identifier");

        x = ParseParameterIdentifier(stream);

        if (!stream.Expect(TokenType.Comma, out Token? _))
            throw new Exception(", expected");

        if (stream.Peek(true, out identifier) && identifier.Type != TokenType.Identifier)
            throw new Exception("Expected identifier");

        y = ParseParameterIdentifier(stream);

        if (!stream.Expect(TokenType.RightParenthesis, out Token? _))
            throw new Exception(") expected");

        return new ParameterCellNode(x, y);
    }

    private static FunctionCallNode ParseFunctionCall(TokenStream stream)
    {
        List<ArgumentNode> arguments = new List<ArgumentNode>();

        if (stream.Expect(TokenType.Identifier, out Token? funcCall))
        {
            stream.Expect(TokenType.Space, out _);
            if (stream.Peek(false, out Token? identifier) && identifier.Type == TokenType.Number || identifier.Type == TokenType.LeftParenthesis
                || identifier.Type == TokenType.Plus || identifier.Type == TokenType.Minus)
            {
                arguments.AddRange(ParseArguments(stream));
            } 

            return new FunctionCallNode(funcCall, arguments);
        }

        throw new Exception("Expected function call");
    }

    private static List<ArgumentNode> ParseArguments(TokenStream stream)
    {
        // Strict rules on space
        List<ArgumentNode> arguments = new List<ArgumentNode>();

        arguments.Add(ParseArgument(stream));

        if (!stream.Expect(TokenType.Space, out Token? _)) return arguments;

        if (stream.Expect(TokenType.Space, out Token? _))
            throw new Exception("Multiple spaces in expression");

        if (stream.Peek(false, out Token? identifier) && identifier.Type == TokenType.Identifier)
            arguments.AddRange(ParseArguments(stream));

        return arguments;
    }

    private static ArgumentNode ParseArgument(TokenStream stream)
    {
        // Has to be rewritten to accomodate (-2) +2 and expressions
        if (stream.Peek(false, out Token? argId) && argId.Type != TokenType.LeftParenthesis)
            return ParseElement(stream);
        return ParseArgCell(stream);
    }

    private static ArgumentNode ParseElement(TokenStream stream)
    {
        stream.Peek(true, out Token? token);
        switch (token.Type)
        {
            case TokenType.Identifier:
                if (stream.Expect(TokenType.Identifier, out Token? identifier))
                    return new IdentifierNode(identifier);
                break;
            case TokenType.Number:
                if (stream.Expect(TokenType.Number, out Token? number))
                    return new ValueNode(number);
                break;
            case TokenType.LeftBracket:
            case TokenType.RightBracket: // range
                return ParseRange(stream);
            case TokenType.LeftParenthesis: // expression
                return ParseExpression(stream);
        }

        throw new Exception("Argument not identified");
    }

    private static CellNode ParseArgCell(TokenStream stream)
    {
        ExpressionNode x;
        ExpressionNode y;

        stream.Expect(TokenType.LeftParenthesis, out Token? startToken);

        if (stream.Peek(true, out Token? identifier) && identifier.Type != TokenType.Identifier)
            throw new Exception("Expected identifier");

        x = ParseExpression(stream);

        if (!stream.Expect(TokenType.Comma, out Token? commaToken))
            throw new Exception(", expected");

        if (stream.Peek(true, out identifier) && identifier.Type != TokenType.Identifier)
            throw new Exception("Expected identifier");

        y = ParseExpression(stream);

        if (!stream.Expect(TokenType.RightParenthesis, out Token? endToken))
            throw new Exception(") expected");

        return new CellNode(startToken, endToken, commaToken, x, y);
    }

    private static ExpressionNode ParseExpression(TokenStream stream)
    {
        throw new NotImplementedException();
    }

    private static RangeNode ParseRange(TokenStream stream)
    {
        throw new NotImplementedException();
    }

    private static RulesNode ParseRules(TokenStream stream)
    {
        throw new NotImplementedException();
    }

    private static GivensNode ParseGivens(TokenStream stream)
    {
        throw new NotImplementedException();
    }
}