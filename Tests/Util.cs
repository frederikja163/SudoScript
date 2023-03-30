using SudoScript.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal static class Util
    {
        private static readonly IReadOnlyList<IRule> _oneRuleRules = new List<IRule>() { new OneRule() };

        public static Unit CreateRow(int y)
        {
            List<CellReference> cells = new List<CellReference>();
            for (int x = 1; x <= 9; x++)
            {
                cells.Add(new CellReference(x, y));
            }
            return new Unit(cells, _oneRuleRules);
        }

        public static Unit CreateColumn(int x)
        {
            List<CellReference> cells = new List<CellReference>();
            for (int y = 1; y <= 9; y++)
            {
                cells.Add(new CellReference(x, y));
            }
            return new Unit(cells, _oneRuleRules);
        }

        public static Unit CreateBox(int x, int y)
        {
            List<CellReference> cells = new List<CellReference>();
            for (int x1 = 0; x1 < 3; x1++)
            {
                for (int y1 = 0; y1 < 3; y1++)
                {
                    cells.Add(new CellReference(x + x1, y + y1));
                }
            }
            return new Unit(cells, _oneRuleRules);
        }

        public static Board CreateStandardEmpty()
        {
            List<Cell> allCells = new List<Cell>();
            List<Unit> units = new List<Unit>();

            for (int x = 1; x <= 9; x++)
            {
                for (int y = 1; y <= 9; y++)
                {
                    allCells.Add(new Cell(x, y));
                }
            }

            units.Add(CreateRow(1));
            units.Add(CreateRow(2));
            units.Add(CreateRow(3));
            units.Add(CreateRow(4));
            units.Add(CreateRow(5));
            units.Add(CreateRow(6));
            units.Add(CreateRow(7));
            units.Add(CreateRow(8));
            units.Add(CreateRow(9));

            units.Add(CreateColumn(1));
            units.Add(CreateColumn(2));
            units.Add(CreateColumn(3));
            units.Add(CreateColumn(4));
            units.Add(CreateColumn(5));
            units.Add(CreateColumn(6));
            units.Add(CreateColumn(7));
            units.Add(CreateColumn(8));
            units.Add(CreateColumn(9));

            units.Add(CreateBox(1, 1));
            units.Add(CreateBox(1, 4));
            units.Add(CreateBox(1, 7));
            units.Add(CreateBox(4, 1));
            units.Add(CreateBox(4, 4));
            units.Add(CreateBox(4, 7));
            units.Add(CreateBox(7, 1));
            units.Add(CreateBox(7, 4));
            units.Add(CreateBox(7, 7));

            return new Board(allCells, units);
        }
    }
}
