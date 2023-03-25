using System.Xml.Linq;

namespace SudoScript.Ast;

public abstract class UnitStatementNode : IAstNode
{
    protected UnitStatementNode(UnitNode parent)
    {
        Parent = parent;
    }

    IAstNode IAstNode.Parent => Parent;
    public UnitNode Parent { get; }

    public abstract IEnumerable<IAstNode> Children();

    public abstract bool Equals(IAstNode? other);
}

public sealed class UnitNode : UnitStatementNode
{
    public UnitNode(UnitNode parent,
        string? name,
        List<UnitStatementNode> unitStatements,
        List<ParameterNode> parameters)
        : base(parent)
    {
        Name = name;
        UnitStatements = unitStatements;
        Parameters = parameters;
    }

    public string? Name { get; }

    public IReadOnlyList<UnitStatementNode> UnitStatements { get; }
    public IReadOnlyList<ParameterNode> Parameters { get; }

    public override IEnumerable<IAstNode> Children()
    {
        return UnitStatements.OfType<IAstNode>().Union(Parameters);
    }

    public override bool Equals(IAstNode? other)
    {
        return other is UnitNode node &&
                node.Parent.Equals(Parent) &&
                node.Name == Name &&
                node.Children().SequenceEqual(UnitStatements);
    }
}

public abstract class ParameterNode : IAstNode
{
    protected ParameterNode(UnitNode parent)
    {
        Parent = parent;
    }

    IAstNode IAstNode.Parent => Parent;

    public UnitNode Parent { get; }

    public abstract IEnumerable<IAstNode> Children();

    public abstract bool Equals(IAstNode? other);
}

public sealed class ParameterCellNode : ParameterNode
{
    public ParameterCellNode(UnitNode parent, ParameterIdentifierNode x, ParameterIdentifierNode y) : base(parent)
    {
        X = x;
        Y = y;
    }

    public ParameterIdentifierNode X { get; }
    public ParameterIdentifierNode Y { get; }

    public override IEnumerable<IAstNode> Children()
    {
        yield return X;
        yield return Y;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is ParameterCellNode node &&
                node.Parent.Equals(Parent) &&
                node.X.Equals(X) &&
                node.Y.Equals(Y);
    }
}

public sealed class ParameterIdentifierNode : ParameterNode
{
    public ParameterIdentifierNode(UnitNode parent, string name) : base(parent)
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
        return other is ParameterIdentifierNode node &&
            node.Name == Name;
    }
}