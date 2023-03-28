namespace SudoScript.Ast;

public interface IAstNode: IEquatable<IAstNode>
{
    public IAstNode? Parent { get; }
    public IEnumerable<IAstNode> Children();
}
