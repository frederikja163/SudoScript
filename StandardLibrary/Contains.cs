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
        Cell? cellsWithCandidate = null;
        foreach (Cell cell in unit.Cells())
        {
            if (cell.HasCandidate(_number))
            {
                cellsWithCandidate = cell;
                if (cellsWithCandidate is null)
                {
                    return false;
                }
            }
        }
        if (cellsWithCandidate is not null)
        {
            cellsWithCandidate.Digit = _number;
            return true;
        }

        return false;
    }

    public bool ValidateRules(Unit unit)
    {
        return unit.Cells().Any(c => c.HasCandidate(_number));
    }

    public override string ToString()
    {
        return $"{nameof(Contains)} {_number}";
    }
}
