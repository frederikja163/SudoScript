using SudoScript.Core.Ast;

namespace SudoScript.Core;

public static class Parser {
    /*
    
     TODO:
        - in method ParseGivensStatement - Missing check for space token
        - in method ParseArgument - Rewrite to enable parsing of expressions wrapped in leftParentheses
        - embed txt files in tests
        - expressions
        - code cleanup
        - github comments
     
     */
    public static ProgramNode ParseProgram(string src) => ParseProgram(Tokenizer.GetStream(src));

    public static ProgramNode ParseProgram(StreamReader reader) => ParseProgram(Tokenizer.GetStream(reader));

    private static ProgramNode ParseProgram(TokenStream stream) 
    {
        // Starts parsing by creating an implicit UnitNode containing entire program.
        return new ProgramNode(new UnitNode(null, ParseUnitStatements(stream), new()));
    }
    private static List<UnitStatementNode> ParseUnitStatements(TokenStream stream) 
    {
        List<UnitStatementNode> children = new List<UnitStatementNode>();

        // While next token is not the last rightbrace of the program, continues to parse UnitStatements
        if (stream.HasNonSpecialNext && stream.Peek(true, out Token? rightBrace) && rightBrace.Type != TokenType.RightBrace) 
        {
            // Here, a single UnitStatement is parsed.
            children.Add(ParseUnitStatement(stream));

            if (!stream.HasNext || (stream.Peek(true, out rightBrace) && rightBrace.Type == TokenType.RightBrace))
            {
                return children;
            }

            stream.Expect(TokenType.Newline, out _);

            // Recursively parses all subsequent UnitStatement nodes.
            children.AddRange(ParseUnitStatements(stream));
        }
        // When all UnitStatement nodes have been read, they are returned
        return children;
    }

    private static UnitStatementNode ParseUnitStatement(TokenStream stream) 
    {
        // Decides how to parse Unit depending on its type.
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

    private static UnitNode ParseUnit(TokenStream stream) 
    {
        List<ParameterNode> paramChildren = new List<ParameterNode>();
        stream.Expect(TokenType.Unit, out _);
        Token? unitName = null;

        // If unit has a name, it will check for parameters.
        if (stream.Peek(true, out Token? name) && name.Type == TokenType.Identifier)
        {
            stream.Expect(TokenType.Identifier, out unitName);

            // If a name identifier is followed by another identifier, it will parse them as parameters.
            if (stream.Peek(true, out Token? parameter) && parameter.Type == TokenType.Identifier)
            {
                paramChildren.AddRange(ParseParameters(stream));
            }
        }

        // Unit body must be wrapped in braces, so throws an error if this is not the case.
        if (!stream.Expect(TokenType.LeftBrace, out _))
        {
            throw new Exception("{ expected");
        }

        // If it reads a rightBrace before any UnitStatements, it will return an empty UnitNode.
        if (stream.Peek(true, out Token? rightBrace) && rightBrace.Type == TokenType.RightBrace)
        {
            return new UnitNode(unitName, new(), paramChildren);
        }

        // Parses UnitStatements within the unit, an returns it when reading a rightBrace.
        UnitNode unitNode = new UnitNode(unitName, ParseUnitStatements(stream), paramChildren);
        if (stream.Expect(TokenType.RightBrace, out _))
        {
            return unitNode;
        }

        // Throws error if unit is not closed with rightBrace.
        throw new Exception("} expected");
    }

    private static List<ParameterNode> ParseParameters(TokenStream stream) 
    {
        List<ParameterNode> paramChildren = new List<ParameterNode>();
        
        // While leftBrace is not read, it will keep reading parameters. 
        while (stream.Peek(true ,out Token? endToken) && endToken.Type != TokenType.LeftBrace) 
        {
            if (!stream.Expect(TokenType.Space, out Token _))
            {
                throw new Exception("Parameter are expected to be separated by space");
            }

            // Checks whether the parameter is an identifier or a cell and parses them acordingly
            if (stream.Peek(true, out Token? leftParenthesis) && leftParenthesis.Type == TokenType.LeftParenthesis)
            {
                paramChildren.Add(ParseParameterCell(stream));
            }

            if (stream.Peek(true, out Token? identifier) && identifier.Type == TokenType.Identifier)
            {
                paramChildren.Add(ParseParameterIdentifier(stream));
            }
        }

        return paramChildren;
    }

    private static ParameterIdentifierNode ParseParameterIdentifier(TokenStream stream) 
    {
        if (!stream.Expect(TokenType.Identifier, out Token? identifier))
        {
            throw new Exception("Expected identifier");
        }

        return new ParameterIdentifierNode(identifier);
    }

    private static ParameterCellNode ParseParameterCell(TokenStream stream) 
    {
        ParameterIdentifierNode x;
        ParameterIdentifierNode y;

        // Parses LeftParenthesis.
        stream.Expect(TokenType.LeftParenthesis, out _);

        if (stream.Peek(true, out Token? identifier) && identifier.Type != TokenType.Identifier)
        {
            throw new Exception("Expected identifier");
        }
        
        // Parses x.
        x = ParseParameterIdentifier(stream);

        // x and y coordinates are comma separated.
        if (!stream.Expect(TokenType.Comma, out _))
        {
            throw new Exception(", expected");
        }

        if (stream.Peek(true, out identifier) && identifier.Type != TokenType.Identifier)
        {
            throw new Exception("Expected identifier");
        }

        // Parses y.
        y = ParseParameterIdentifier(stream);

        // Parses RightParenthesis
        if (!stream.Expect(TokenType.RightParenthesis, out _))
        {
            throw new Exception(") expected");
        }

        return new ParameterCellNode(x, y);
    }

    private static FunctionCallNode ParseFunctionCall(TokenStream stream) 
    {
        List<ArgumentNode> arguments = new List<ArgumentNode>();
        Token unionToken = new Token(TokenType.Identifier, "Union", "", 0, 0, "");

        // If functionCall is an identifier, checks for arguments.
        if (stream.Expect(TokenType.Identifier, out Token? functionCall)) 
        {
            stream.Expect(TokenType.Space, out _);

            // If functionCall has arguments, parses them
            if (stream.Peek(false, out Token? identifier) && identifier.Type == TokenType.Number || identifier?.Type == TokenType.LeftParenthesis || identifier?.Type == TokenType.Plus || identifier?.Type == TokenType.Minus) 
            {
                arguments.AddRange(ParseArguments(stream));
            }

            return new FunctionCallNode(functionCall, arguments);
        }

        // If functionCall is a cell, it will parse it accordingly.
        if(stream.Peek(true, out Token? union) && union.Type == TokenType.LeftParenthesis) 
        {
            arguments.Add(ParseCell(stream));

            return new FunctionCallNode(unionToken, arguments);
        }

        throw new Exception("Expected function call");
    }

    private static List<ArgumentNode> ParseArguments(TokenStream stream) 
    {
        // Parses single argument.
        List<ArgumentNode> arguments = new List<ArgumentNode>
        {
            ParseArgument(stream)
        };

        // Since arguments are separated by spaces, checks for space after first argument.
        if (!stream.Expect(TokenType.Space, out _))
        {
            return arguments;
        }

        // Throws error if there is more than one space.
        if (stream.Expect(TokenType.Space, out _))
        {
            throw new Exception("Multiple spaces in expression");
        }

        // If more arguments follow, parse them recursively.
        if (stream.Peek(false, out Token? identifier) && (identifier.Type == TokenType.Identifier || identifier.Type == TokenType.LeftParenthesis))
        {
            arguments.AddRange(ParseArguments(stream));
        }

        return arguments;
    }

    private static ArgumentNode ParseArgument(TokenStream stream) 
    {

        // If argument is not a cell, parse it as an element.
        if (stream.Peek(false, out Token? argument) && argument.Type != TokenType.LeftParenthesis)
        {
            return ExpressionParser.Parse(stream, true);
        }
        
        //otherwise, parse as cell
        return ParseCell(stream);
    }

    private static RulesNode ParseRules(TokenStream stream) 
    {
        // Parses rules UnitNode
        if (!stream.Expect(TokenType.Rules, out Token? rulesToken)) 
        { 
            throw new Exception("Expected token 'rules'");
        }

        // the rulesNode needs a startToken and a endToken.
        if (stream.Peek(true, out Token? token) && token.Type != TokenType.LeftBrace)
        {
            throw new Exception("Expected { after 'rules' token");
        }

        stream.Expect(TokenType.LeftBrace, out Token? startToken);

        // Parses the RuleStatements within the rules UnitNode.
        List<FunctionCallNode> children = ParseRuleStatements(stream);

        if (stream.Peek(true, out Token? token1) && token1.Type != TokenType.RightBrace)
        {
            throw new Exception("Expected } to end rules statement");
        }
        
        stream.Expect(TokenType.RightBrace, out Token? endToken);

        return new RulesNode(rulesToken, startToken!, endToken!, children);
    }

    private static List<FunctionCallNode> ParseRuleStatements(TokenStream stream) 
    {
        List<FunctionCallNode> ruleStatements = new List<FunctionCallNode>();

        // While there are more RuleStatement nodes, parses them.
        while (stream.Peek(true, out Token? endToken) && endToken.Type != TokenType.RightBrace)
        {
            ruleStatements.Add(ParseRuleStatement(stream));
        }

        return ruleStatements;
    }

    private static FunctionCallNode ParseRuleStatement(TokenStream stream) 
    {
        // If Rule is not named, throws exception.
        if (stream.Expect(TokenType.Identifier, out Token? nameToken))
        {
            stream.Expect(TokenType.Space, out _);

            // if Rule is empty, return empty functionCall.
            if (stream.Peek(false, out Token? argumentsCheck) && (argumentsCheck.Type == TokenType.Space || argumentsCheck.Type == TokenType.Newline))
            {
                return new FunctionCallNode(nameToken, new());
            }

            // Parse all rule arguments. 
            List<ArgumentNode> ruleArguments = ParseArguments(stream);

            // return FunctionCallNode with named rule and its arguments.
            return new FunctionCallNode(nameToken, ruleArguments);
        }

        throw new NullReferenceException();
    }

    private static GivensNode ParseGivens(TokenStream stream)
    {
        // Parse givens UnitNode
        if (!stream.Expect(TokenType.Givens, out _))
        {
            throw new Exception("Expected token 'givens'");
        }

        // The givens UnitNode must be wrapped in braces.
        if (stream.Peek(true, out Token? token) && token.Type != TokenType.LeftBrace)
        {
            throw new Exception("Expected { after 'givens' token");
        }

        stream.Expect(TokenType.LeftBrace, out _);

        // Parse empty givensNode if givens Unit is empty
        if (stream.Peek(true, out Token? token1) && token1.Type == TokenType.RightBrace) 
        {
            stream.Expect(TokenType.RightBrace, out _);
            return new GivensNode(new List<GivensStatementNode>());
        }

        // Parse body of givens UnitNode
        List<GivensStatementNode> children = ParseGivensStatements(stream);

        stream.Expect(TokenType.RightBrace, out _);

        return new GivensNode(children);
    }

    private static List<GivensStatementNode> ParseGivensStatements(TokenStream stream) 
    {
        List<GivensStatementNode> ruleStatements = new List<GivensStatementNode>();

        // While endToken is not reached, parses givenStatements.
        while (stream.Peek(true, out Token? endToken) && endToken.Type != TokenType.RightBrace)
        {
            ruleStatements.Add(ParseGivensStatement(stream));
        }

        return ruleStatements;
    }

    private static GivensStatementNode ParseGivensStatement(TokenStream stream)
    {
        // Since the grammar for a given is 'Cell space Element', checks for start of cell syntax.
        if (stream.Peek(true, out Token? token) && token.Type != TokenType.LeftParenthesis)
        {
            throw new Exception("expected a cell");
        }

        CellNode cellNode = ParseCell(stream);
        ExpressionNode value = ExpressionParser.Parse(stream);

        return new GivensStatementNode(cellNode, value);
    }

    private static CellNode ParseCell(TokenStream stream) 
    {
        ExpressionNode x;
        ExpressionNode y;

        // Cell grammar is as follows: Cell -> leftParenthesis Expression comma Expression rightParenthesis.
        // Parses LeftParenthesis.
        if(stream.Expect(TokenType.LeftParenthesis, out Token? startToken))
        {
            if (stream.Peek(true, out Token? identifier) && identifier.Type != TokenType.Number)
            {
                throw new Exception("Expected identifier");
            }

            // Parses x
            x = ExpressionParser.Parse(stream);

            // Since x and y are comma separated, parses comma.
            if (!stream.Expect(TokenType.Comma, out Token? commaToken))
            {
                throw new Exception(", expected");
            }

            if (stream.Peek(true, out identifier) && identifier.Type != TokenType.Number)
            {
                throw new Exception("Expected identifier");
            }

            // Parses y.
            y = ExpressionParser.Parse(stream);

            // Parses RightParenthesis.
            if (!stream.Expect(TokenType.RightParenthesis, out Token? endToken))
            {
                throw new Exception(") expected");
            }

            return new CellNode(startToken, endToken, commaToken, x, y);
        }

        throw new NullReferenceException();
    }
}