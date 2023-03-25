namespace SudoScript.Ast;

public sealed class GivensNode : UnitStatementNode
{
    public GivensNode(UnitNode parent, IReadOnlyList<GivensStatementNode> givensStatements) : base(parent)
    {
        GivensStatements = givensStatements;
    }

    IReadOnlyList<GivensStatementNode> GivensStatements { get; }

    public override IEnumerable<IAstNode> Children()
    {
        return GivensStatements;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is GivensNode node &&
            node.Equals(this);
    }
}

public sealed class GivensStatementNode : IAstNode
{
    public GivensStatementNode(GivensNode parent, CellNode cell, ExpressionNode digit)
    {
        Cell = cell;
        Digit = digit;
        Parent = parent;
    }

    public CellNode Cell { get; }
    public ExpressionNode Digit { get; }

    IAstNode IAstNode.Parent => Parent;
    public GivensNode Parent { get; }

    public IEnumerable<IAstNode> Children()
    {
        yield return Cell;
        yield return Digit;
    }

    public bool Equals(IAstNode? other)
    {
        return other is GivensStatementNode node &&
            node.Cell.Equals(Cell) &&
            node.Digit.Equals(Digit);
    }
}
