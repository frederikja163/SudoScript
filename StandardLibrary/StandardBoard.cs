using SudoScript.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public class StandardBoard : Unit
    {
        public StandardBoard(int x, int y) : base(InitCells(x, y), new List<IRule> { })
        {
        }
        public StandardBoard() : base(InitCells(1, 1), new List<IRule> { })
        {
        }

        private static List<Unit> InitCells(int x, int y)
        {
            List<Unit> units = new List<Unit>();
            for (int i = 1; i <= 9; i++)
            {
                units.Add(new Row(i));
                units.Add(new Column(i));
            }
            units.Add(new Box((x - 1) + 1, (y - 1) + 1));
            units.Add(new Box((x - 1) + 1, (y - 1) + 4));
            units.Add(new Box((x - 1) + 1, (y - 1) + 7));
            units.Add(new Box((x - 1) + 4, (y - 1) + 1));
            units.Add(new Box((x - 1) + 4, (y - 1) + 4));
            units.Add(new Box((x - 1) + 4, (y - 1) + 7));
            units.Add(new Box((x - 1) + 7, (y - 1) + 1));
            units.Add(new Box((x - 1) + 7, (y - 1) + 4));
            units.Add(new Box((x - 1) + 7, (y - 1) + 7));
            return units;
        }
    }
}
