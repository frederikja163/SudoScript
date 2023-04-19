﻿using SudoScript.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public class Sum : IRule
    {
        public Sum(int sumVal)
        {
            SumVal = sumVal;
        }

        internal int SumVal { get; private init; }


        public bool EliminateCandidates(Unit unit)
        {
            bool somethingEliminated = false;
            int currentTotal = 0;
            foreach (Cell cell in unit.Cells())
            {
                currentTotal += cell.Digit;
            }
            int remainder = SumVal - currentTotal;
            //Make a list of empty cells
            IList<Cell> emptyCells = new List<Cell>();
            foreach (Cell cell in unit.Cells()) 
            { 
                if (cell.Digit == Cell.EmptyDigit) 
                { 
                    emptyCells.Add(cell); 
                } 
            }
            //If there is only one empty cell
            if (emptyCells.Count == 1)
            {
                emptyCells[0].EliminateCandidate(a => a != remainder);
            }
            //Test each candidate in each cell to see if it is possible to get the sum
            foreach (Cell cell in emptyCells)
            {
                //Create a list of all other cells
                List<Cell> otherCells = emptyCells.ToList();
                otherCells.Remove(cell);
                foreach (int candidate in cell.Candidates())
                {
                    if (!RecursiveValidSumSearch(remainder, otherCells.ToList(), candidate))
                    {
                        cell.EliminateCandidate(candidate);
                        somethingEliminated = true;
                    }
                }
            }
            return somethingEliminated;
        }

        private bool RecursiveValidSumSearch(int remainder, List<Cell> emptyCells, int runningSum)
        {
            Cell currentCell = emptyCells[0];
            emptyCells.RemoveAt(0);
            if (emptyCells.Count == 0) //End recursivity if there are no more cells
            {
                foreach (int candidate in currentCell.Candidates())
                {
                    if (!(runningSum + candidate => remainder))
                    {
                        return true;
                    }
                }
                return false;
            }
            foreach (int candidate in currentCell.Candidates()) //Tests each candidate
            {
                if (runningSum + candidate > remainder) 
                { 
                    return false; 
                }
                if (RecursiveValidSumSearch(remainder, emptyCells.ToList(), runningSum + candidate))
                {
                    return true;
                }
            }
            return false;
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
