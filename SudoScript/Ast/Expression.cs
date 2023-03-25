namespace SudoScript.Ast;

public abstract class ExpressionNode : ArgumentNode
{
    protected ExpressionNode(FunctionCallNode parent) : base(parent)
    {
    }
}

public sealed class RangeNode : ExpressionNode
{
    public RangeNode(FunctionCallNode parent,
        ExpressionNode minimumExpression,
        ExpressionNode maximumExpression,
        bool isMinInclusive,
        bool isMaxInclusive) : base(parent)
    {
        MinimumExpression = minimumExpression;
        MaximumExpression = maximumExpression;
        IsMinInclusive = isMinInclusive;
        IsMaxInclusive = isMaxInclusive;
    }

    public ExpressionNode MinimumExpression { get; }
    public ExpressionNode MaximumExpression { get; }
    public bool IsMinInclusive { get; }
    public bool IsMaxInclusive { get; }

    public override IEnumerable<IAstNode> Children()
    {
        yield return MinimumExpression;
        yield return MaximumExpression;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is RangeNode node &&
                node.MinimumExpression.Equals(MinimumExpression) &&
                node.MaximumExpression.Equals(MaximumExpression) &&
                node.IsMinInclusive == IsMinInclusive &&
                node.IsMaxInclusive == IsMaxInclusive;
    }
}


public enum BinaryType
{
    Plus,
    Minus,
    Multiply,
    Mod,
    Power
}

public sealed class BinaryNode : ExpressionNode
{
    public BinaryNode(FunctionCallNode parent, BinaryType binaryType, ExpressionNode left, ExpressionNode right) : base(parent)
    {
        BinaryType = binaryType;
        Left = left;
        Right = right;
    }

    public ExpressionNode Left { get; }
    public ExpressionNode Right { get; }
    public BinaryType BinaryType { get; }

    public override IEnumerable<IAstNode> Children()
    {
        yield return Left;
        yield return Right;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is BinaryNode node &&
                node.Left.Equals(Left) &&
                node.Right.Equals(Right) &&
                node.BinaryType == BinaryType;
    }
}

public enum UnaryType
{
    Plus,
    Minus
}

public sealed class UnaryNode : ExpressionNode
{
    public UnaryNode(FunctionCallNode parent, UnaryType unaryType, ExpressionNode expression) : base(parent)
    {
        UnaryType = unaryType;
        Expression = expression;
    }

    public ExpressionNode Expression { get; }

    public UnaryType UnaryType { get; }

    public override IEnumerable<IAstNode> Children()
    {
        yield return Expression;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is UnaryNode node &&
            node.Expression.Equals(Expression) &&
            node.UnaryType == UnaryType;
    }
}

public sealed class IdentifierNode : ExpressionNode
{
    public IdentifierNode(FunctionCallNode parent, string name) : base(parent)
    {
        Name = name;
    }

    public string Name { get; }

    public override IEnumerable<IAstNode> Children()
    {
        yield break;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is IdentifierNode node &&
            node.Name == Name;
    }
}