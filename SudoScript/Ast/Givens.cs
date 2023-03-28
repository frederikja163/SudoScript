namespace SudoScript.Ast;

public sealed class GivensNode : UnitStatementNode
{
    public GivensNode(IReadOnlyList<GivensStatementNode> givensStatements)
    {
        GivensStatements = givensStatements;
        foreach (GivensStatementNode givensStatement in GivensStatements)
        {
            givensStatement.Parent = this;
        }
    }

    public IReadOnlyList<GivensStatementNode> GivensStatements { get; }

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
    public GivensStatementNode(CellNode cell, ExpressionNode digit)
    {
        Cell = cell;
        Cell.Parent = this;
        Digit = digit;
        Digit.Parent = this;
    }

    public CellNode Cell { get; }
    public ExpressionNode Digit { get; }

    IAstNode? IAstNode.Parent => Parent;
    public GivensNode? Parent { get; internal set; }

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
