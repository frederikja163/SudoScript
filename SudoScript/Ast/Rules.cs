namespace SudoScript.Ast;

public sealed class RulesNode : UnitStatementNode
{
    public RulesNode(UnitNode parent) : base(parent)
    {
    }

    public IReadOnlyList<FunctionCallNode> FunctionCalls { get; }

    public override IEnumerable<IAstNode> Children()
    {
        return FunctionCalls;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is RulesNode node &&
            FunctionCalls.SequenceEqual(node.FunctionCalls);
    }
}
