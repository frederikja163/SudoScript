﻿using SudoScript.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    internal class Row : Unit
    {
        public Row(IReadOnlyList<CellReference> cells, IReadOnlyList<IRule> rules) : base(cells, rules)
        {
            {
                throw new Exception("Invalid Row placement");
            }
            List<CellReference> rowCells = new List<CellReference>();
            for (int i = 0; i < 9; i++) 
            {
                rowCells.Add(new CellReference(cells[0].X , cells[0].Y));
            }
            cells = rowCells.ToArray();

        }

        private bool ValidateRow(IReadOnlyList<CellReference> cells)
        {
            if (cells == null) { return false; }
            if (cells.Count != 9) { return false; }
            for (int i = 0; i < cells.Count-1; i++) //We do not want to check the last cell since it has already been compared to the previous cell
            {
                if (cells[i].X+1 != cells[i + 1].X) //Test if the cells are adjacent
                {
                    return false;
                }
                if (cells[i].Y != cells[i + 1].Y)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
