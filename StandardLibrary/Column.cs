using SudoScript.Core.Data;

namespace StandardLibrary
{
    public class Column : Unit
    {
        public Column(int x) : base(InitCells(x, 1), new List<IRule> { new OneRule() }) //Y is assumed to be 1
        {
        }
        public Column(int x, int y) : base(InitCells(x, y), new List<IRule> { new OneRule() })
        {
        }

        private static List<CellReference> InitCells(int x, int y)
        {
            List<CellReference> columnCells = new List<CellReference>();
            for (int i = y; i < y + 9; i++)
            {
                columnCells.Add(new CellReference(x, i));
            }
            return columnCells;
        }
    }
}
