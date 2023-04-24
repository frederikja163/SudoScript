using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudoScript.Data;

namespace StandardLibrary
{
    public class Odd : IRule
    {
        public bool EliminateCandidates(Unit unit)
        {
            bool somethingEliminated = false;
            foreach (Cell cell in unit.Cells())
            {
                foreach (int candidate in cell.Candidates())
                {
                    if (candidate % 2 == 0)
                    {
                        somethingEliminated = true;
                        cell.EliminateCandidate(candidate);
                    }
                }
            }
            return somethingEliminated;
        }

        public bool ValidateRules(Unit unit)
        {
            foreach (Cell cell in unit.Cells())
            {
                if (cell.Digit % 2 == 0 && cell.Digit != Cell.EmptyDigit)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
