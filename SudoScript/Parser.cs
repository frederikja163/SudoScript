using SudoScript.Ast;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace SudoScript;

public static class Parser {
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

        if (stream.HasNext() && stream.Peek(out Token? RightBrace) && rightBrace != TokenType.RightBrace) 
        {
            children.Add(ParseUnitStatement(stream));
            if (!stream.HasNext() || stream.Peek().Type == TokenType.RightBrace)
                return children;

            stream.Accept(TokenType.Newline, out Token? _);
            children.AddRange(ParseUnitStatements(stream));
        }

        return children;
    }

    private static UnitStatementNode ParseUnitStatement(TokenStream stream)
    {
        if (stream.MoveNext())
        {
            switch (stream.Peek().Type)
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

        stream.Accept(TokenType.Unit, out Token? _);
        if (stream.Accept(TokenType.Identifier, out Token? identifier))
        {
            if(stream.Peek().Type == TokenType.Identifier)
                paramChildren.AddRange(ParseParameters(stream));
        }

        if (!stream.Accept(TokenType.LeftBrace, out Token? _)) 
            throw new Exception("{ expected");


        UnitNode unitNode = new UnitNode(identifier,ParseUnitStatements(stream), paramChildren);
        if(stream.Accept(TokenType.RightBrace, out Token? rightBrace))
        {
                return unitNode;
        }

        throw new Exception("} expected");
    }

    private static List<ParameterNode> ParseParameters(TokenStream stream)
    {
        List<ParameterNode> paramChildren = new List<ParameterNode>();
        // Check if it is a identifier or cell param
        while(!stream.Accept(TokenType.LeftBrace, out Token _))
        {
            if(stream.Peek().Type == TokenType.LeftParenthesis)
            {
                paramChildren.Add(ParseParameterCell(stream));
            }

            if(stream.Peek().Type == TokenType.Identifier) 
                paramChildren.Add(ParseParameterIdentifier(stream));

        }

        return paramChildren;
    }

    private static ParameterIdentifierNode ParseParameterIdentifier(TokenStream stream)
    {
        if (!stream.Accept(TokenType.Identifier, out Token? identifier)) 
            throw new Exception("Expected identifier");
            
        return new ParameterIdentifierNode(identifier);
    }

    private static ParameterCellNode ParseParameterCell(TokenStream stream)
    {
        ParameterIdentifierNode x;
        ParameterIdentifierNode y;
            
        stream.Accept(TokenType.LeftParenthesis, out Token? _);

        if (!(stream.Peek().Type == TokenType.Identifier)) 
            throw new Exception("Expected identifier");

        x = ParseParameterIdentifier(stream);

        if (!stream.Accept(TokenType.Comma, out Token? _))
            throw new Exception(", expected");

        if (stream.Peek().Type != TokenType.Identifier) 
            throw new Exception("Expected identifier");

        y = ParseParameterIdentifier(stream);

        if (!stream.Accept(TokenType.RightParenthesis, out Token? _))
            throw new Exception(") expected");

        return new ParameterCellNode(x, y);
    }

    private static FunctionCallNode ParseFunctionCall(TokenStream stream)
    {
        List<ArgumentNode> arguments = new List<ArgumentNode>();

        if(stream.Accept(TokenType.Identifier, out Token? identifier))
        {
            if(stream.Peek().Type == TokenType.Identifier)
            {
                arguments.AddRange(ParseArguments(stream));
            }

            return new FunctionCallNode(identifier, arguments);
        }

        throw new Exception("Expected function call");
    }

    private static List<ArgumentNode> ParseArguments(TokenStream stream)
    {
        // Strict rules on space
        List<ArgumentNode> arguments= new List<ArgumentNode>();

        arguments.Add(ParseArgument(stream));

        if (!stream.Accept(TokenType.Space, out Token? _)) return arguments;

        if (stream.Accept(TokenType.Space, out Token? _)) 
            throw new Exception("Multiple spaces in expression");

        if (stream.Peek().Type == TokenType.Identifier) arguments.AddRange(ParseArguments(stream));

        return arguments;
    }

    private static ArgumentNode ParseArgument(TokenStream stream)
    {
        if (stream.Peek().Type != TokenType.LeftParenthesis)
            return ParseElement(stream);
        return ParseArgCell(stream);
    }

    private static ArgumentNode ParseElement(TokenStream stream)
    {
        switch(stream.Peek().Type)
        {
            case TokenType.Identifier:
                if(stream.Accept(TokenType.Identifier, out Token? identifier)) 
                    return new IdentifierNode(identifier);
                break;
            case TokenType.Number:
                if (stream.Accept(TokenType.Number, out Token? number)) 
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

        stream.Accept(TokenType.LeftParenthesis, out Token startToken);

        if (stream.Peek().Type != TokenType.Identifier)
            throw new Exception("Expected identifier");

        x = ParseExpression(stream);

        if (!stream.Accept(TokenType.Comma, out Token commaToken))
            throw new Exception(", expected");

        if (stream.Peek().Type != TokenType.Identifier)
            throw new Exception("Expected identifier");

        y = ParseExpression(stream);

        if (!stream.Accept(TokenType.RightParenthesis, out Token endToken))
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