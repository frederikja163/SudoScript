using SudoScript.Ast;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace SudoScript;

public static class Parser {
    // Consider implementing this in a style similar to: https://github.com/frederikja163/TuringComplete
    public static ProgramNode ParseProgram(string src) => ParseProgram(Tokenizer.GetStream(src));

    public static ProgramNode ParseProgram(StreamReader reader) => ParseProgram(Tokenizer.GetStream(reader));

    private static ProgramNode ParseProgram(TokenStream stream) {
        //return new ProgramNode(ParseUnit(stream));
        return new ProgramNode(new UnitNode(null, ParseUnitStatements(stream), new()));
    }
    private static List<UnitStatementNode> ParseUnitStatements(TokenStream stream) {
        List<UnitStatementNode> children = new List<UnitStatementNode>();

        if (stream.HasNext && stream.Peek(true, out Token? rightBrace) && rightBrace.Type != TokenType.RightBrace) {
            children.Add(ParseUnitStatement(stream));
            if (!stream.HasNext || (stream.Peek(true, out rightBrace) && rightBrace.Type == TokenType.RightBrace))
                return children;

            stream.Expect(TokenType.Newline, out Token? _);
            children.AddRange(ParseUnitStatements(stream));
        }

        return children;
    }

    private static UnitStatementNode ParseUnitStatement(TokenStream stream) {
        // TODO: Has next includes special characters
        if (stream.Peek(true, out Token? token))
        {
            switch (token.Type) {
                case TokenType.Unit:
                    return ParseUnit(stream);
                case TokenType.Identifier:
                case TokenType.LeftParenthesis:
                    return ParseFunctionCall(stream);
                case TokenType.Rules:
                    return ParseRules(stream);
                case TokenType.Givens:
                    return ParseGivens(stream);
                default:
                    throw new Exception("syntax error");
            } 
        }
        else
        {
            throw new NullReferenceException();
        }
    }

    private static UnitNode ParseUnit(TokenStream stream) {
        List<ParameterNode> paramChildren = new List<ParameterNode>();

        stream.Expect(TokenType.Unit, out Token? unit);
        Token? identifier = null;

        if (stream.Peek(true, out Token? identifier1) && identifier1.Type == TokenType.Identifier) // left off here, tokenizer expects identifier but gets leftBrace and still moves on
        {
            stream.Expect(TokenType.Identifier, out identifier);
            if (stream.Peek(true, out Token? paramId) && paramId.Type == TokenType.Identifier)
            {
                paramChildren.AddRange(ParseParameters(stream));
            }
        }
        if (!stream.Expect(TokenType.LeftBrace, out Token? _))
            throw new Exception("{ expected");

        if (stream.Peek(true, out Token? id) && id.Type == TokenType.RightBrace)
            return new UnitNode(identifier, new(), paramChildren);

        UnitNode unitNode = new UnitNode(identifier, ParseUnitStatements(stream), paramChildren);

        if (stream.Expect(TokenType.RightBrace, out Token? rightBrace)) {
            return unitNode;
        }

        throw new Exception("} expected");
    }

    private static List<ParameterNode> ParseParameters(TokenStream stream) {
        List<ParameterNode> paramChildren = new List<ParameterNode>();
        // Check if it is a identifier or cell param
        while (stream.Peek(true ,out Token? endToken) && endToken.Type != TokenType.LeftBrace) {
            if (!stream.Expect(TokenType.Space, out Token _))
                throw new Exception("Params are expected to be separated by space");

            if (stream.Peek(true, out Token? leftParenthesis) && leftParenthesis.Type == TokenType.LeftParenthesis)
                paramChildren.Add(ParseParameterCell(stream));

            if (stream.Peek(true, out Token? identifier) && identifier.Type == TokenType.Identifier)
                paramChildren.Add(ParseParameterIdentifier(stream));

        }

        return paramChildren;
    }

    private static ParameterIdentifierNode ParseParameterIdentifier(TokenStream stream) {
        if (!stream.Expect(TokenType.Identifier, out Token? identifier))
            throw new Exception("Expected identifier");

        return new ParameterIdentifierNode(identifier);
    }

    private static ParameterCellNode ParseParameterCell(TokenStream stream) {
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

    private static FunctionCallNode ParseFunctionCall(TokenStream stream) {
        List<ArgumentNode> arguments = new List<ArgumentNode>();
        Token unionToken = new Token(TokenType.Identifier, "union", "", 0, 0, "");

        if (stream.Expect(TokenType.Identifier, out Token? funcCall)) {
            stream.Expect(TokenType.Space, out _);
            if (stream.Peek(false, out Token? identifier) && identifier.Type == TokenType.Number || identifier.Type == TokenType.LeftParenthesis
                || identifier.Type == TokenType.Plus || identifier.Type == TokenType.Minus) {
                arguments.AddRange(ParseArguments(stream));
            }

            return new FunctionCallNode(funcCall, arguments);
        }

        if(stream.Peek(true, out Token? union) && union.Type == TokenType.LeftParenthesis) 
        {
            arguments.Add(ParseCell(stream));
            return new FunctionCallNode(unionToken, arguments);
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

        if (stream.Peek(false, out Token? identifier) && (identifier.Type == TokenType.Identifier || identifier.Type == TokenType.LeftParenthesis))
            arguments.AddRange(ParseArguments(stream));

        return arguments;
    }

    private static ArgumentNode ParseArgument(TokenStream stream) {
        // Has to be rewritten to accomodate (-2) +2 and expressions
        if (stream.Peek(false, out Token? argId) && argId.Type != TokenType.LeftParenthesis)
            return ParseElement(stream);
        return ParseCell(stream);
    }

    private static ArgumentNode ParseElement(TokenStream stream) {
        stream.Peek(true, out Token? token);
        switch (token.Type) {
            case TokenType.Identifier:
                if (stream.Expect(TokenType.Identifier, out Token? identifierToken))
                    return new IdentifierNode(identifierToken);
                break;
            case TokenType.Minus: // maybe handle error if no number after minus
            case TokenType.Plus:
            case TokenType.Number:
                stream.Expect(TokenType.Number, out Token? valueToken);
                return new ValueNode(valueToken);
            case TokenType.LeftBracket:
            case TokenType.RightBracket: // range
                return ParseRange(stream);
            case TokenType.LeftParenthesis: // expression
                return ParseExpression(stream);
        }

        throw new Exception("Argument not identified");
    }

    private static CellNode ParseArgCell(TokenStream stream) {
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

    private static ExpressionNode ParseExpression(TokenStream stream) {
        // Expression ::= Expression plus Term
        // how do we know when the expression ends and the term begins?

        // 5+6+7*2+4 -> (5+6)+((7*2)+4)
        // Expression -> Expression plus Term -> Expression plus Term Plus Term

        // 5+9+3*8+7+1

        //stream.Peek(true, out Token? token);
        //switch (token.Type)
        //{
        //    case TokenType.LeftBracket: case TokenType.RightBracket:
        //        ParseRange(stream);
        //        break;
        //    case TokenType.Minus: // -2+4
        //        // unary minus might be broken... -(2+4) contra (-2)+4
        //        stream.Expect(TokenType.Minus, out Token? minusToken);
        //        return new UnaryNode(token, UnaryType.Minus, ParseExpression(stream));
        //    case TokenType.Plus:
        //        stream.Expect(TokenType.Plus, out Token? plusToken);
        //        return new UnaryNode(token, UnaryType.Plus, ParseExpression(stream));
        //    case TokenType.Number:
        //        stream.Expect(TokenType.Number, out Token? numberToken);
        //        if (stream.Peek(false, out Token? space)) return new ValueNode(numberToken);
        //        return ParseBinary(stream, numberToken);
        //}

        // Temp solution
        stream.Expect(TokenType.Number, out Token? value);
        return new ValueNode(value);
    }

    /*
    private static ExpressionNode ParseBinary(TokenStream stream, Token? leftToken) 
    { 
        stream.Peek(true, out Token? operatorToken);
        BinaryType binaryType = new();

        switch (operatorToken.Type)
        {
            case TokenType.Plus: binaryType = BinaryType.Plus; break;
            case TokenType.Minus: binaryType = BinaryType.Minus; break;
            case TokenType.Multiply: binaryType = BinaryType.Multiply; break;
            case TokenType.Mod: binaryType = BinaryType.Mod; break;
            case TokenType.Power: binaryType = BinaryType.Power; break;
            default: throw new Exception("Expected operator");
        }

        return new BinaryNode(operatorToken, binaryType, new ValueNode(leftToken), ParseExpression(stream));
    }

    */

    private static ExpressionNode ParseTerm(TokenStream stream) {
        throw new NotImplementedException();
    }

    private static ExpressionNode ParseUnary(TokenStream stream) {
        throw new NotImplementedException();
    }

    private static ExpressionNode ParseFactor(TokenStream stream) {
        throw new NotImplementedException();
    }

    private static ExpressionNode ParseFunctionElement(TokenStream stream) {
        throw new NotImplementedException();
    }

    private static RangeNode ParseRange(TokenStream stream) {
        throw new NotImplementedException();
    }

    private static RulesNode ParseRules(TokenStream stream) {
        stream.Expect(TokenType.Rules, out Token? rulesToken);
        if (stream.Peek(true, out Token? token) && token.Type != TokenType.LeftBrace) throw new Exception("Expected { after rules keyword");
        stream.Expect(TokenType.LeftBrace, out Token? startToken);

        List<FunctionCallNode> children = ParseRuleStatements(stream);

        if (stream.Peek(true, out Token? token1) && token1.Type != TokenType.RightBrace) throw new Exception("Expected } to end rules statement");
        stream.Expect(TokenType.RightBrace, out Token? endToken);
        return new RulesNode(rulesToken, startToken, endToken, children);
    }

    private static List<FunctionCallNode> ParseRuleStatements(TokenStream stream) {
        List<FunctionCallNode> ruleStatements = new List<FunctionCallNode>();

        while (stream.Peek(true, out Token? endToken) && endToken.Type != TokenType.RightBrace) {
            ruleStatements.Add(ParseRuleStatement(stream));
        }
        return ruleStatements;
    }

    private static FunctionCallNode ParseRuleStatement(TokenStream stream) 
    {
        stream.Expect(TokenType.Identifier, out Token? nameToken);
        stream.Expect(TokenType.Space, out _);

        if (stream.Peek(false, out Token? argumentsCheck) && (argumentsCheck.Type == TokenType.Space || argumentsCheck.Type == TokenType.Newline))
            return new FunctionCallNode(null, new());
        List<ArgumentNode> ruleArguments = ParseFuncArguments(stream);
        return new FunctionCallNode(nameToken, ruleArguments);
    }

    private static GivensNode ParseGivens(TokenStream stream)
    {
        stream.Expect(TokenType.Givens, out _);
        if (stream.Peek(true, out Token? token) && token.Type != TokenType.LeftBrace) throw new Exception("Expected { after rules keyword");
        stream.Expect(TokenType.LeftBrace, out _);

        if (stream.Peek(true, out Token? token1) && token1.Type == TokenType.RightBrace) 
        {
            stream.Expect(TokenType.RightBrace, out _);
            return new GivensNode(new List<GivensStatementNode>());
        }
        List<GivensStatementNode> children = ParseGivensStatements(stream);
        stream.Expect(TokenType.RightBrace, out _);
        return new GivensNode(children);
    }

    private static List<GivensStatementNode> ParseGivensStatements(TokenStream stream) {
        List<GivensStatementNode> ruleStatements = new List<GivensStatementNode>();

        while (stream.Peek(true, out Token? endToken) && endToken.Type != TokenType.RightBrace)
        {
            ruleStatements.Add(ParseGivensStatement(stream));
        }
        return ruleStatements;
    }

    private static GivensStatementNode ParseGivensStatement(TokenStream stream)
    {
        if (stream.Peek(true, out Token? token) && token.Type != TokenType.LeftParenthesis)
            throw new Exception("expected a cell");

        CellNode cellNode = ParseCell(stream);
        ExpressionNode value = ParseExpression(stream);
        return new GivensStatementNode(cellNode, value);
    }

    private static List<ArgumentNode> ParseFuncArguments(TokenStream stream) {
        // Strict rules on space
        List<ArgumentNode> arguments = new List<ArgumentNode>();

        arguments.Add(ParseFuncArgument(stream));

        if (!stream.Expect(TokenType.Space, out Token? _)) return arguments;

        if (stream.Expect(TokenType.Space, out Token? _))
            throw new Exception("Multiple spaces in expression");

        if (stream.Peek(false, out Token? identifier) && (identifier.Type == TokenType.Identifier || identifier.Type == TokenType.LeftParenthesis))
            arguments.AddRange(ParseFuncArguments(stream));

        return arguments;
    }

    private static ArgumentNode ParseFuncArgument(TokenStream stream) {
        // Has to be rewritten to accomodate (-2) +2 and expressions
        if (stream.Peek(false, out Token? argId) && argId.Type != TokenType.LeftParenthesis)
            return ParseElement(stream);
        return ParseCell(stream);
    }

    private static CellNode ParseCell(TokenStream stream) {
        ExpressionNode x;
        ExpressionNode y;

        stream.Expect(TokenType.LeftParenthesis, out Token? startToken);

        if (stream.Peek(true, out Token? identifier) && identifier.Type != TokenType.Number)
            throw new Exception("Expected identifier");

        x = ParseExpression(stream);

        if (!stream.Expect(TokenType.Comma, out Token? commaToken))
            throw new Exception(", expected");

        if (stream.Peek(true, out identifier) && identifier.Type != TokenType.Number)
            throw new Exception("Expected identifier");

        y = ParseExpression(stream);

        if (!stream.Expect(TokenType.RightParenthesis, out Token? endToken))
            throw new Exception(") expected");

        return new CellNode(startToken, endToken, commaToken, x, y);
    }
}