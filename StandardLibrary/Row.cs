using SudoScript.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public class Row : Unit
    {
        public Row(int y) : base(InitCells(1, y), new List<IRule> { new OneRule() }) //X is assumed to be 1
        {
        }
        public Row(int x, int y) : base(InitCells(x, y), new List<IRule> { new OneRule() })
        {
        }

        private static List<CellReference> InitCells(int x, int y)
        {
            List<CellReference> rowCells = new List<CellReference>();
            for (int i = x; i < x + 9; i++)
            {
                rowCells.Add(new CellReference(i, y));
            }
            return rowCells;
        }
    }
}
