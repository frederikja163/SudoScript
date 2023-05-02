namespace SudoScript.Core.Ast;

public abstract class ExpressionNode : ArgumentNode
{
}

public sealed class RangeNode : ExpressionNode
{
    public RangeNode(Token startToken, Token endToken, Token semiColonToken,
        ExpressionNode minimumExpression,
        ExpressionNode maximumExpression,
        bool isMinInclusive,
        bool isMaxInclusive)
    {
        StartToken = startToken;
        EndToken = endToken;
        SemiColonToken = semiColonToken;
        MinimumExpression = minimumExpression;
        MinimumExpression.Parent = this;
        MaximumExpression = maximumExpression;
        MaximumExpression.Parent = this;
        IsMinInclusive = isMinInclusive;
        IsMaxInclusive = isMaxInclusive;
    }

    public Token StartToken { get; }
    public Token EndToken { get; }
    public Token SemiColonToken { get; }
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
                node.StartToken.Equals(StartToken) &&
                node.EndToken.Equals(EndToken) &&
                node.SemiColonToken.Equals(SemiColonToken) &&
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
    public BinaryNode(Token operatorToken, BinaryType binaryType, ExpressionNode left, ExpressionNode right)
    {
        OperatorToken = operatorToken;
        BinaryType = binaryType;
        Left = left;
        Left.Parent = this;
        Right = right;
        Right.Parent = this;
    }

    public Token OperatorToken { get; }
    public BinaryType BinaryType { get; }
    public ExpressionNode Left { get; }
    public ExpressionNode Right { get; }

    public override IEnumerable<IAstNode> Children()
    {
        yield return Left;
        yield return Right;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is BinaryNode node &&
                node.OperatorToken.Equals(OperatorToken) &&
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
    public UnaryNode(Token operatorToken, UnaryType unaryType, ExpressionNode expression)
    {
        OperatorToken = operatorToken;
        UnaryType = unaryType;
        Expression = expression;
        Expression.Parent = this;
    }

    public Token OperatorToken { get; }
    public UnaryType UnaryType { get; }
    public ExpressionNode Expression { get; }

    public override IEnumerable<IAstNode> Children()
    {
        yield return Expression;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is UnaryNode node &&
                node.OperatorToken == OperatorToken &&
                node.Expression.Equals(Expression) &&
                node.UnaryType.Equals(UnaryType);
    }
}

public sealed class IdentifierNode : ExpressionNode
{
    public IdentifierNode(Token nameToken)
    {
        NameToken = nameToken;
    }

    public Token NameToken { get; }

    public override IEnumerable<IAstNode> Children()
    {
        yield break;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is IdentifierNode node &&
            node.NameToken.Equals(NameToken);
    }
}

public sealed class ValueNode : ExpressionNode
{
    public ValueNode(Token valueToken)
    {
        ValueToken = valueToken;
    }

    public Token ValueToken { get; }

    public override IEnumerable<IAstNode> Children()
    {
        yield break;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is ValueNode node &&
            node.ValueToken.Equals(ValueToken);
    }
}