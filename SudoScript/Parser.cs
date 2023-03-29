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

        while (stream.HasNext() && stream.Peek().Type != TokenType.RightBrace)
        {
            children.Add(ParseUnitStatement(stream));
        }

        return children;
    }
    private static UnitStatementNode ParseUnitStatement(TokenStream stream)
    {
        //List<IAstNode> children = new List<IAstNode>() { ParseUnitStatement(stream) };

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
        if (stream.MoveNext())
        {
            switch (stream.Peek().Type)
            {
                case TokenType.Identifier:
                    stream.Accept(TokenType.Identifier, out Token? identifier);
                    break;
                case TokenType.LeftBrace:
                    break; // left off here, return something good
                default:
                    throw new Exception("{ expected");
            }
        }

        throw new Exception("{ expected");
    }
    private static FunctionCallNode ParseFunctionCall(TokenStream stream)
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
