using SudoScript.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    internal class Row : Unit
    {
        public Row(int y) : base(InitCells(1, y), new IRule[] { new OneRule() }) //X is assumed to be 1
        {
        }
        public Row(int x, int y) : base(InitCells(x, y), new IRule[] { new OneRule() })
        {
        }

        private static IReadOnlyList<CellReference> InitCells(int x, int y)
        {
            List<CellReference> rowCells = new List<CellReference>();
            for (int i = x; i <= i + 9; i++)
            {
                rowCells.Add(new CellReference(i, y));
            }
            return rowCells.ToArray();
        }
    }
}
