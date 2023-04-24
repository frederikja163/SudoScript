using SudoScript.Ast;

namespace SudoScript;

public static class Parser {
    /*
    
     TODO:
        - embed txt files in tests
        - comments
        - expressions
        - code cleanup
        - github comments
     
     */

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
            {
                return children;
            }

            stream.Expect(TokenType.Newline, out _);
            children.AddRange(ParseUnitStatements(stream));
        }

        return children;
    }

    private static UnitStatementNode ParseUnitStatement(TokenStream stream) 
    {
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

        stream.Expect(TokenType.Unit, out Token? unit);
        Token? identifier = null;

        if (stream.Peek(true, out Token? identifier1) && identifier1.Type == TokenType.Identifier)
        {
            stream.Expect(TokenType.Identifier, out identifier);
            if (stream.Peek(true, out Token? paramId) && paramId.Type == TokenType.Identifier)
            {
                paramChildren.AddRange(ParseParameters(stream));
            }
        }

        if (!stream.Expect(TokenType.LeftBrace, out _))
        {
            throw new Exception("{ expected");
        }

        if (stream.Peek(true, out Token? id) && id.Type == TokenType.RightBrace)
        {
            return new UnitNode(identifier, new(), paramChildren);
        }

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
        while (stream.Peek(true ,out Token? endToken) && endToken.Type != TokenType.LeftBrace) 
        {
            if (!stream.Expect(TokenType.Space, out Token _))
            {
                throw new Exception("Params are expected to be separated by space");
            }

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

        stream.Expect(TokenType.LeftParenthesis, out _);

        if (stream.Peek(true, out Token? identifier) && identifier.Type != TokenType.Identifier)
        {
            throw new Exception("Expected identifier");
        }

        x = ParseParameterIdentifier(stream);

        if (!stream.Expect(TokenType.Comma, out _))
        {
            throw new Exception(", expected");
        }

        if (stream.Peek(true, out identifier) && identifier.Type != TokenType.Identifier)
        {
            throw new Exception("Expected identifier");
        }

        y = ParseParameterIdentifier(stream);

        if (!stream.Expect(TokenType.RightParenthesis, out _))
        {
            throw new Exception(") expected");
        }

        return new ParameterCellNode(x, y);
    }

    private static FunctionCallNode ParseFunctionCall(TokenStream stream) 
    {
        List<ArgumentNode> arguments = new List<ArgumentNode>();
        Token unionToken = new Token(TokenType.Identifier, "union", "", 0, 0, "");

        if (stream.Expect(TokenType.Identifier, out Token? funcCall)) 
        {
            stream.Expect(TokenType.Space, out _);
            if (stream.Peek(false, out Token? identifier) && identifier.Type == TokenType.Number || identifier?.Type == TokenType.LeftParenthesis || identifier?.Type == TokenType.Plus || identifier?.Type == TokenType.Minus) 
            {
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
        List<ArgumentNode> arguments = new List<ArgumentNode>();

        arguments.Add(ParseArgument(stream));

        if (!stream.Expect(TokenType.Space, out _))
        {
            return arguments;
        }

        if (stream.Expect(TokenType.Space, out _))
        {
            throw new Exception("Multiple spaces in expression");
        }

        if (stream.Peek(false, out Token? identifier) && (identifier.Type == TokenType.Identifier || identifier.Type == TokenType.LeftParenthesis))
        {
            arguments.AddRange(ParseArguments(stream));
        }

        return arguments;
    }

    private static ArgumentNode ParseArgument(TokenStream stream) 
    {
        //TODO: Has to be rewritten to accomodate (-2) +2 and expressions
        if (stream.Peek(false, out Token? argId) && argId.Type != TokenType.LeftParenthesis)
        {
            return ParseElement(stream);
        }

        return ParseCell(stream);
    }

    private static ArgumentNode ParseElement(TokenStream stream) 
    {
        stream.Peek(true, out Token? token);
        switch (token?.Type) {
            case TokenType.Identifier:
                if (stream.Expect(TokenType.Identifier, out Token? identifierToken))
                {
                    return new IdentifierNode(identifierToken);
                }

                break;
            case TokenType.Minus:
            case TokenType.Plus:
            case TokenType.Number:
                if(stream.Expect(TokenType.Number, out Token? valueToken))
                {
                    return new ValueNode(valueToken);
                }

                throw new NullReferenceException();
            case TokenType.LeftBracket:
            case TokenType.RightBracket:
                return ParseRange(stream);
            case TokenType.LeftParenthesis:
                return ParseExpression(stream);
        }

        throw new Exception("Argument not identified");
    }

    private static ExpressionNode ParseExpression(TokenStream stream) 
    {
        if(stream.Expect(TokenType.Number, out Token? value)) // temp
        {
            return new ValueNode(value);
        }

        IEnumerable<Token> tokens = stream.Next(false, p => p == TokenType.Newline || p == TokenType.Space);
        Token? last = null;

        last = tokens.Last(p => p.Type == TokenType.Plus || p.Type == TokenType.Minus);

        int lastIndex = tokens.ToList().LastIndexOf(last);

        if(lastIndex == 0 || tokens.ElementAt(lastIndex-1)?.Type != TokenType.Number)
        {
            return ParseTerm(tokens);
        }

        if (tokens.Any(p => p.Type != TokenType.Plus && p.Type != TokenType.Minus))
        {
            return ParseTerm(tokens);
        }

        BinaryType type = last.Type == TokenType.Plus ? BinaryType.Plus : BinaryType.Minus;

        Split(tokens.ToArray(), lastIndex, out Token[] expression, out Token[] term);

        return new BinaryNode(last, type, ParseExpression(expression), ParseTerm(term));
    }

    private  static ExpressionNode ParseExpression(IEnumerable<Token> tokens) 
    {
        if (tokens.Any(p => p.Type != TokenType.Plus && p.Type != TokenType.Minus))
        {
            return ParseTerm(tokens);
        }

        throw new NotImplementedException();
    }

    public static void Split<T>(T[] array, int index, out T[] first, out T[] second)
    {
        first = array.Take(index-1).ToArray();
        second = array.Skip(index).ToArray();
    }

    private static ExpressionNode ParseTerm(IEnumerable<Token> tokens) 
    {
        if (tokens.Any(p => p.Type != TokenType.Mod && p.Type != TokenType.Multiply))
        {
            return ParseUnary(tokens);
        }

        Token last = tokens.Last(p => p.Type == TokenType.Mod || p.Type == TokenType.Multiply);
        int lastIndex = tokens.ToList().LastIndexOf(last);
        BinaryType type = last.Type == TokenType.Mod ? BinaryType.Mod : BinaryType.Multiply;

        Split(tokens.ToArray(), lastIndex, out Token[] term, out Token[] unary);

        return new BinaryNode(last, type, ParseTerm(term), ParseUnary(unary));
    }

    private static ExpressionNode ParseUnary(IEnumerable<Token> tokens) 
    {
        if (tokens.Any(p => p.Type != TokenType.Plus && p.Type != TokenType.Minus))
        {
            return ParseFactor(tokens);
        }

        throw new NotImplementedException();
    }

    private static ExpressionNode ParseFactor(IEnumerable<Token> tokens) 
    {
        if (tokens.Any(p => p.Type != TokenType.Power))
        {
            return ParseFunctionElement(tokens);
        }

        throw new NotImplementedException();
    }

    private static ExpressionNode ParseFunctionElement(IEnumerable<Token> tokens) 
    {
        if(tokens.First().Type is TokenType.Number)
        {
            return new ValueNode(tokens.First());
        }

        throw new NotImplementedException();
    }

    private static RangeNode ParseRange(TokenStream stream) 
    {
        throw new NotImplementedException();
    }

    private static RulesNode ParseRules(TokenStream stream) 
    {
        if (!stream.Expect(TokenType.Rules, out Token? rulesToken)) 
        { 
            throw new Exception("Expected token 'rules'");
        }

        if (stream.Peek(true, out Token? token) && token.Type != TokenType.LeftBrace)
        {
            throw new Exception("Expected { after rules keyword");
        }

        if (!stream.Expect(TokenType.LeftBrace, out Token? startToken))
        {
            throw new Exception("Exptected left brace");
        } 

        List<FunctionCallNode> children = ParseRuleStatements(stream);

        if (stream.Peek(true, out Token? token1) && token1.Type != TokenType.RightBrace)
        {
            throw new Exception("Expected } to end rules statement");
        }

        if (!stream.Expect(TokenType.RightBrace, out Token? endToken))
        {
            throw new Exception("Expected right brace");
        } 

        return new RulesNode(rulesToken, startToken, endToken, children);
    }

    private static List<FunctionCallNode> ParseRuleStatements(TokenStream stream) 
    {
        List<FunctionCallNode> ruleStatements = new List<FunctionCallNode>();

        while (stream.Peek(true, out Token? endToken) && endToken.Type != TokenType.RightBrace)
        {
            ruleStatements.Add(ParseRuleStatement(stream));
        }

        return ruleStatements;
    }

    private static FunctionCallNode ParseRuleStatement(TokenStream stream) 
    {
        if (stream.Expect(TokenType.Identifier, out Token? nameToken))
        {
            stream.Expect(TokenType.Space, out _);

            if (stream.Peek(false, out Token? argumentsCheck) && (argumentsCheck.Type == TokenType.Space || argumentsCheck.Type == TokenType.Newline))
            {
                return new FunctionCallNode(new Token(TokenType.Identifier, "", "", 0, 0, ""), new());
            }

            List<ArgumentNode> ruleArguments = ParseFuncArguments(stream);

            return new FunctionCallNode(nameToken, ruleArguments);
        }

        throw new NullReferenceException();
    }

    private static GivensNode ParseGivens(TokenStream stream)
    {
        stream.Expect(TokenType.Givens, out _);
        if (stream.Peek(true, out Token? token) && token.Type != TokenType.LeftBrace)
        {
            throw new Exception("Expected { after givens keyword");
        }

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

    private static List<GivensStatementNode> ParseGivensStatements(TokenStream stream) 
    {
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
        {
            throw new Exception("expected a cell");
        }

        CellNode cellNode = ParseCell(stream);
        ExpressionNode value = ParseExpression(stream);

        return new GivensStatementNode(cellNode, value);
    }

    private static List<ArgumentNode> ParseFuncArguments(TokenStream stream) 
    {
        List<ArgumentNode> arguments = new List<ArgumentNode>();

        arguments.Add(ParseFuncArgument(stream));

        if (!stream.Expect(TokenType.Space, out _))
        { 
            return arguments; 
        }

        if (stream.Expect(TokenType.Space, out _))
        {
            throw new Exception("Multiple spaces in expression");
        }

        if (stream.Peek(false, out Token? identifier) && (identifier.Type == TokenType.Identifier || identifier.Type == TokenType.LeftParenthesis))
        {
            arguments.AddRange(ParseFuncArguments(stream));
        }

        return arguments;
    }

    private static ArgumentNode ParseFuncArgument(TokenStream stream) 
    {
        //TODO: Has to be rewritten to accomodate (-2) +2 and expressions
        if (stream.Peek(false, out Token? argId) && argId.Type != TokenType.LeftParenthesis)
        {
            return ParseElement(stream);
        }

        return ParseCell(stream);
    }

    private static CellNode ParseCell(TokenStream stream) 
    {
        ExpressionNode x;
        ExpressionNode y;

        if(stream.Expect(TokenType.LeftParenthesis, out Token? startToken))
        {
            if (stream.Peek(true, out Token? identifier) && identifier.Type != TokenType.Number)
            {
                throw new Exception("Expected identifier");
            }

            x = ParseExpression(stream);

            if (!stream.Expect(TokenType.Comma, out Token? commaToken))
            {
                throw new Exception(", expected");
            }

            if (stream.Peek(true, out identifier) && identifier.Type != TokenType.Number)
            {
                throw new Exception("Expected identifier");
            }

            y = ParseExpression(stream);

            if (!stream.Expect(TokenType.RightParenthesis, out Token? endToken))
            {
                throw new Exception(") expected");
            }

            return new CellNode(startToken, endToken, commaToken, x, y);
        }

        throw new NullReferenceException();
    }
}