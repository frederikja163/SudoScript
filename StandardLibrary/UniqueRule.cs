using SudoScript.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public sealed class UniqueRule : IRule
    {
        public bool EliminateCandidates(Unit unit)
        {
            bool somethingEliminated = false;
            //Get a list of all digits in unit
            List<int> digits = new List<int>();
            foreach (Cell cell in unit.Cells())
            {
                if (!digits.Contains(cell.Digit)) //If them digit is not already in list
                {
                    digits.Add(cell.Digit);
                }
            }
            //Get a list of all empty cell in this unit
            List<Cell> emptyCells = new List<Cell>();
            foreach (Cell cell in unit.Cells())
            {
                if (cell.Digit == Cell.EmptyDigit)
                {
                    emptyCells.Add(cell);
                }
            }
            //Eliminate candidates from empty cells based on list of digits
            foreach (Cell cell in emptyCells)
            {
                if (cell.EliminateCandidate(digits))
                {
                    somethingEliminated = true;
                }
                
            }
            return somethingEliminated;
        }

        public bool ValidateRules(Unit unit)
        {
            //Get a list of all digits within the unit
            List<int> digits = new List<int>();
            foreach (Cell cell in unit.Cells())
            {
                digits.Add(cell.Digit);
            }
            //Sort list
            digits.Sort();
            //Check for dublicates
            for (int i = 1; i < digits.Count; i++)
            {
                if (digits[i] != 0) {
                    if (digits[i] == digits[i - 1])
                    {
                        return false;
                    }
                }
            }
            //The loop has checked all digits without returning
            return true;
        }
    }
}
