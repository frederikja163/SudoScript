﻿using SudoScript.Core.Data;

namespace StandardLibrary;

public class Even : IRule
{
    public bool EliminateCandidates(Unit unit)
    {
        bool somethingEliminated = false;
        foreach (Cell cell in unit.Cells())
        {
            if (cell.Digit == Cell.EmptyDigit && cell.EliminateCandidate(c => c % 2 != 0))
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
            if (cell.Digit != Cell.EmptyDigit && cell.Digit % 2 != 0)
            {
                return false;
            }
        }
        return true;
    }
    
    public override string ToString()
    {
        return nameof(Even);
    }
}
