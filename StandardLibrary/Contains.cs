using SudoScript.Core.Data;

namespace StandardLibrary;

public class Contains : IRule
{
    public Contains(int number)
    {
        Number = number;
    }

    internal int Number { get; private init; }
    public bool EliminateCandidates(Unit unit)
    {
        int numCandidates = 0;
        foreach (Cell cell in unit.Cells())
        {
            if (cell.Candidates().Contains(Number))
            {
                numCandidates++;
            }
            if (numCandidates > 1)
            {
                return false;
            }

        }
        if (numCandidates == 1)
        {
            foreach (Cell cell in unit.Cells())
            {
                if (cell.Candidates().Contains(Number))
                {
                    cell.EliminateCandidate(a => a != Number);
                    return true;
                }
            }
        }
        return false;
    }

    public bool ValidateRules(Unit unit)
    {
        bool contains = false;
        foreach (Cell cell in unit.Cells())
        {
            if (cell.Digit == Number)
            {
                contains = true;
            }
        }
        return contains;
    }
}
