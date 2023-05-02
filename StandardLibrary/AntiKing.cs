using SudoScript.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public class AntiKing : IRule
    {
        public bool EliminateCandidates(Unit unit)
        {
            bool somethingEliminated = false;
            foreach(Cell cell in unit.Cells())
            {
                List<Cell> seenCells = listSeenCells(cell, unit);
                foreach (Cell seenCell in seenCells)
                {
                    if (cell.EliminateCandidate(seenCell.Digit))
                    {
                        somethingEliminated = true;
                    }
                }
            }
            return somethingEliminated;
        }

        public bool ValidateRules(Unit unit)
        {
            foreach (Cell cell in unit.Cells())
            {
                if (cell.Digit == Cell.EmptyDigit)
                {
                    continue;
                }
                List<Cell> seenCells = listSeenCells(cell, unit);
                foreach (Cell seenCell in seenCells)
                {
                    if (cell.Digit == seenCell.Digit)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private static readonly int[,] coordinates =
        {
            {-1, 1}, 
            {0, 1},
            {1, 1},
            {1, 0},
            {-1, 0},
            {-1, -1},
            {0, -1},
            {1, -1}
        };
        private List<Cell> listSeenCells(Cell cell, Unit unit)
        {
            List<Cell> list = new List<Cell>();
            if (unit.Board is null)
            {
                throw new Exception(this.ToString() + "- unit.Board is null");
            }
            for (int i = 0; i < coordinates.GetLength(0); i++)
            {
                if (unit.Board.Cells().Any(c => c.X == cell.X + coordinates[i,0] && c.Y == cell.Y + coordinates[i, 1]))
                {
                    list.Add(unit.Board[cell.X + coordinates[i, 0], cell.Y + coordinates[i, 1]]);
                }
            }
            return list;
        }



    }
}
