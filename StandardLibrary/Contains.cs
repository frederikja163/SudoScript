using SudoScript.Core.Data;

namespace StandardLibrary;

public class Contains : IRule
{
    public Contains(int number)
    {
        _number = number;
    }

    private readonly int _number;

    public bool EliminateCandidates(Unit unit)
    {
        int numCandidates = 0;
        foreach (Cell cell in unit.Cells())
        {
            if (cell.HasCandidate(_number))
            {
                numCandidates++;
                if (numCandidates > 1)
                {
                    return false;
                }
            }
        }

        if (numCandidates == 1)
        {
            foreach (Cell cell in unit.Cells())
            {
                if (cell.HasCandidate(_number))
                {
                    cell.EliminateCandidate(a => a != _number);
                    return true;
                }
            }
        }

        return false;
    }

    public bool ValidateRules(Unit unit)
    {
        foreach (Cell cell in unit.Cells())
        {
            if (cell.Digit == _number)
            {
                return true;
            }
        }

        return false;
    }

    public override string ToString()
    {
        return $"{nameof(Contains)} {_number}";
    }
}
