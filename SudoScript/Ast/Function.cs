namespace SudoScript.Ast;

public sealed class FunctionCallNode : UnitStatementNode
{
    public FunctionCallNode(Token name, List<ArgumentNode> arguments)
    {
        Name = name;
        Arguments = arguments;
        foreach (ArgumentNode argument in arguments)
        {
            argument.Parent = this;
        }
    }

    public Token Name { get; }
    public IReadOnlyList<ArgumentNode> Arguments { get; }

    public override IEnumerable<IAstNode> Children()
    {
        return Arguments;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is FunctionCallNode node &&
            node.Name == Name &&
            node.Arguments.SequenceEqual(Arguments);
    }
}

public abstract class ArgumentNode : IAstNode
{
    public IAstNode? Parent { get; internal set; }

    public abstract IEnumerable<IAstNode> Children();

    public abstract bool Equals(IAstNode? other);
}

public sealed class CellNode : ArgumentNode
{
    public CellNode(Token startToken, Token endToken, Token commaToken, ExpressionNode x, ExpressionNode y)
    {
        StartToken = startToken;
        EndToken = endToken;
        CommaToken = commaToken;
        X = x;
        X.Parent = this;
        Y = y;
        Y.Parent = this;
    }

    public Token StartToken { get; }
    public Token EndToken { get; }
    public Token CommaToken { get; }
    public ExpressionNode X { get; }
    public ExpressionNode Y { get; }

    public override IEnumerable<IAstNode> Children()
    {
        yield return X;
        yield return Y;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is CellNode node &&
            node.StartToken.Equals(StartToken) &&
            node.EndToken.Equals(EndToken) &&
            node.CommaToken.Equals(CommaToken) &&
            node.X.Equals(X) &&
            node.Y.Equals(Y);
    }
}
