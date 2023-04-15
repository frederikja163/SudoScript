using SudoScript.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    internal class Sum : IRule
    {
        internal int SumVal { get; set; }


        public bool EliminateCandidates(Unit unit)
        {
            bool somethingEliminated = false;
            int currentTotal = 0;
            foreach (Cell cell in unit.Cells())
            {
                currentTotal += cell.Digit;
            }
            int numberOfEmptyCells = 0;
            foreach (Cell cell in unit.Cells()) { if (cell.Digit == Cell.EmptyDigit) { numberOfEmptyCells++; } } //Count the number of empty cells

            if ((SumVal - currentTotal) <= 8 + numberOfEmptyCells)
            {

            }

            return somethingEliminated;
        }



        public bool ValidateRules(Unit unit)
        {
            int counter = 0;
            foreach (Cell cell in unit.Cells()) { counter += cell.Digit; }
            if (counter == SumVal) { return true; }
            return false;
        }
    }
}
