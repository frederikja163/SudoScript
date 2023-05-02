namespace SudoScript.Core.Ast;

public sealed class RulesNode : UnitStatementNode
{
    public RulesNode(Token rulesToken, Token startToken, Token endToken, IReadOnlyList<FunctionCallNode> functionCallNode)
    {
        RulesToken = rulesToken;
        StartToken = startToken;
        EndToken = endToken;
        FunctionCalls = functionCallNode;
        foreach (FunctionCallNode functionCall in FunctionCalls)
        {
            functionCall.Parent = this;
        }
    }

    public Token RulesToken { get; }
    public Token StartToken { get; }
    public Token EndToken { get; }
    public IReadOnlyList<FunctionCallNode> FunctionCalls { get; }

    public override IEnumerable<IAstNode> Children()
    {
        return FunctionCalls;
    }

    public override bool Equals(IAstNode? other)
    {
        return other is RulesNode node &&
            node.RulesToken.Equals(RulesToken) &&
            node.StartToken.Equals(StartToken) &&
            node.EndToken.Equals(EndToken) &&
            node.FunctionCalls.SequenceEqual(FunctionCalls);
    }
}
