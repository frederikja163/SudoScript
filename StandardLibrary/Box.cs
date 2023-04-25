using SudoScript.Core.Data;

namespace StandardLibrary
{
    public class Box : Unit
    {
        public Box(int x, int y) : base(InitCells(x, y), new List<IRule> { new OneRule() })
        {
        }

        private static List<CellReference> InitCells(int x, int y)
        {
            List<CellReference> cells = new List<CellReference>();
            for (int i = x; i <= x + 2; i++)
            {
                for (int j = y; j <= y + 2; j++)
                {
                    cells.Add(new CellReference(i,j));
                }
            }
            return cells;
        }
    }
}
