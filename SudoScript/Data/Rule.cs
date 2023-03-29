namespace SudoScript.Data;

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
    public abstract void EliminateCandidates(Unit unit);
    public abstract bool ValidateRules(Unit unit);
}

public sealed class OneRule : IRule
{
    public void EliminateCandidates(Unit unit)
    {
        HashSet<int> seenDigits = new HashSet<int>();
        foreach (Cell cell in unit.Cells())
        {
            if (cell.Digit == Cell.EmptyDigit)
            {
                // Remove all seen digits from the candidates of this cell.
                cell.Candidates.RemoveWhere(x => seenDigits.Contains(x));
            }
            else
            {
                if (seenDigits.Contains(cell.Digit))
                {
                    throw new RuleException(unit, cell, $"One rule is not followed, multiple instances of {cell.Digit} found.");
                }
                seenDigits.Add(cell.Digit);
            }
        }
    }

    public bool ValidateRules(Unit unit)
    {
        HashSet<int> seenDigits = new HashSet<int>();
        foreach (Cell cell in unit.Cells())
        {
            if (cell.Digit != Cell.EmptyDigit)
            {
                if (seenDigits.Contains(cell.Digit))
                {
                    return false;
                }
                seenDigits.Add(cell.Digit);
            }
        }
        return true;
    }
}