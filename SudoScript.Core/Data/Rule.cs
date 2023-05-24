namespace SudoScript.Core.Data;

public sealed class RuleException : Exception
{
    public RuleException(Unit unit, Cell cell, string message) : base(message)
    {
        Unit = unit;
        Cell = cell;
    }

    public Unit Unit { get; }
    public Cell Cell { get; }
}

public interface IRule
{
    public abstract bool EliminateCandidates(Unit unit);
    public abstract bool ValidateRules(Unit unit);
}