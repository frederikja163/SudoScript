using SudoScript.Core.Data;

namespace StandardLibrary;

public class Row : Unit
{
    public Row(int y) : this((1, y)) //X is assumed to be 1
    {
    }
    public Row(int x, int y) : this((x, y))
    {
    }
    public Row(CellReference reference) : base(InitCells(reference), new List<IRule> { new OneRule()})
    {
        _origin = reference;
    }

    private readonly CellReference _origin;

    private static List<CellReference> InitCells(CellReference reference)
    {
        List<CellReference> rowCells = new List<CellReference>();
        for (int i = reference.X; i < reference.X + 9; i++)
        {
            rowCells.Add(new CellReference(i, reference.Y));
        }
        return rowCells;
    }

    public override string ToString()
    {
        return base.ToString(nameof(Row) + " " + _origin);
    }
}
