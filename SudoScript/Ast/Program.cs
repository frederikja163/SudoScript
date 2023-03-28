namespace SudoScript.Ast;

public sealed class ProgramNode : IAstNode
{
    public ProgramNode(UnitNode child)
    {
        Child = child;
        Child.Parent = this;
    }

    public IAstNode? Parent => null;

    public UnitNode Child { get; }


    public IEnumerable<IAstNode> Children()
    {
        yield return Child;
    }

    public bool Equals(IAstNode? other)
    {
        return other is ProgramNode node &&
            node.Child.Equals(Child);
    }
}
