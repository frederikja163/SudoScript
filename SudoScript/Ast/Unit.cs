namespace SudoScript.Ast;

public abstract class UnitStatementNode : IAstNode
{
    public IAstNode? Parent { get; internal set; }

    public abstract IEnumerable<IAstNode> Children();

    public abstract bool Equals(IAstNode? other);
}

public sealed class UnitNode : UnitStatementNode
{
    public UnitNode(string? name,
        List<UnitStatementNode> unitStatements,
        List<ParameterNode> parameters)
    {
        Name = name;
        UnitStatements = unitStatements;
        foreach (UnitStatementNode unitStatement in UnitStatements)
        {
            unitStatement.Parent = this;
        }
        Parameters = parameters;
        foreach (ParameterNode parameter in Parameters)
        {
            parameter.Parent = this;
        }
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
                (node.Parent?.Equals(Parent) ?? false) &&
                node.Name == Name &&
                node.Children().SequenceEqual(UnitStatements);
    }
}

public abstract class ParameterNode : IAstNode
{
    public IAstNode? Parent { get; internal set; }

    public abstract IEnumerable<IAstNode> Children();

    public abstract bool Equals(IAstNode? other);
}

public sealed class ParameterCellNode : ParameterNode
{
    public ParameterCellNode(ParameterIdentifierNode x, ParameterIdentifierNode y)
    {
        X = x;
        X.Parent = this;
        Y = y;
        Y.Parent = this;
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
                (node.Parent?.Equals(Parent) ?? false) &&
                node.X.Equals(X) &&
                node.Y.Equals(Y);
    }
}

public sealed class ParameterIdentifierNode : ParameterNode
{
    public ParameterIdentifierNode(string name)
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