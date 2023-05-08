using SudoScript.Core.Data;

namespace StandardLibrary;

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
        // One rule requires all symbols, in order for that it needs a specific size of unit.
        // TODO: Remember to change this if updating amount of symbols.
        // .ToHashSet is needed to ensure all the cells are unique (seperate coordinates)
        if (unit.Cells().ToHashSet().Count() != 9)
        {
            return false;
        }

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

    public override string ToString()
    {
        return nameof(OneRule);
    }
}