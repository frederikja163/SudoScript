namespace SudoScript.Ast;

public sealed class FunctionCallNode : UnitStatementNode
{
    public FunctionCallNode(UnitNode parent, string name, List<ArgumentNode> arguments) : base(parent)
    {
        Name = name;
        Arguments = arguments;
    }

    public string Name { get; }
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
    protected ArgumentNode(FunctionCallNode parent)
    {
        Parent = parent;
    }

    IAstNode IAstNode.Parent => Parent;
    public FunctionCallNode Parent { get; }

    public abstract IEnumerable<IAstNode> Children();

    public abstract bool Equals(IAstNode? other);
}

public sealed class CellNode : ArgumentNode
{
    public CellNode(FunctionCallNode parent, ExpressionNode x, ExpressionNode y) : base(parent)
    {
        X = x;
        Y = y;
    }

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
            node.X.Equals(X) &&
            node.Y.Equals(Y);
    }
}
