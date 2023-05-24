using SudoScript.Core.Data;

namespace StandardLibrary;

public class Column : Unit
{
    public Column(int x) : this((x, 1)) // Y is assumed to be 1.
    {
    }

    public Column(int x, int y) : this((x, y))
    {
    }

    public Column(CellReference reference) : base(InitCells(reference), new List<IRule>() { new OneRule() })
    {
        _origin = reference;
    }

    private readonly CellReference _origin;

    private static List<CellReference> InitCells(CellReference reference)
    {
        List<CellReference> columnCells = new List<CellReference>();
        for (int i = reference.Y; i < reference.Y + 9; i++)
        {
            columnCells.Add(reference with { Y = i });
        }

        return columnCells;
    }

    public override string ToString()
    {
        return base.ToString($"{nameof(Column)} {_origin}");
    }
}