using SudoScript.Core.Data;

namespace StandardLibrary;

public class Odd : IRule
{
    public bool EliminateCandidates(Unit unit)
    {
        bool somethingEliminated = false;
        foreach (Cell cell in unit.Cells())
        {
            if (cell.EliminateCandidate(c => c % 2 != 1))
            {
                somethingEliminated = true;
            }
        }

        return somethingEliminated;
    }

    public bool ValidateRules(Unit unit)
    {
        foreach (Cell cell in unit.Cells())
        {
            if (cell.Digit % 2 != 1)
            {
                return false;
            }
        }
        return true;
    }

    public override string ToString()
    {
        return nameof(Odd);
    }
}
