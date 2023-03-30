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
    public abstract bool EliminateCandidates(Unit unit);
    public abstract bool ValidateRules(Unit unit);
}

public sealed class OneRule : IRule
{
    public bool EliminateCandidates(Unit unit)
    {
        // Get a list of all digits in the unit.
        HashSet<int> digits = unit.Cells()
            .Select(c => c.Digit)
            .Where(d => d != Cell.EmptyDigit)
            .ToHashSet();

        bool somethingEliminated = false;
        foreach (Cell cell in unit.Cells())
        {
            if (cell.Digit == Cell.EmptyDigit)
            {
                // Remove all seen digits from the candidates of this cell.
                if (cell.EliminateCandidate(digits))
                {
                    somethingEliminated = true;
                }
            }
        }
        return somethingEliminated;
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