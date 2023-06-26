using SudoScript.Core.Data;

namespace StandardLibrary;

public class Box : Unit
{
    public Box(int x, int y) : this((x, y))
    {
        
    }
    
    public Box(CellReference reference) : base(InitCells(reference), new List<IRule> { new OneRule() })
    {
        _origin = reference;
    }

    private readonly CellReference _origin;

    private static List<CellReference> InitCells(CellReference reference)
    {
        List<CellReference> cells = new List<CellReference>();
        for (int i = reference.X; i <= reference.X + 2; i++)
        {
            for (int j = reference.Y; j <= reference.Y + 2; j++)
            {
                cells.Add(new CellReference(i,j));
            }
        }
        return cells;
    }

    public override string ToString()
    {
        return base.ToString($"{nameof(Box)} {_origin}");
    }
}
