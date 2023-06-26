using SudoScript.Core.Data;

namespace StandardLibrary;

public sealed class Battenburg : Unit
{
    private readonly CellReference _bottomLeft;
    public Cell BottomLeft => Board[_bottomLeft.X, _bottomLeft.Y];
    public Cell TopLeft => Board[_bottomLeft.X + 1, _bottomLeft.Y];
    public Cell BottomRight => Board[_bottomLeft.X, _bottomLeft.Y + 1];
    public Cell TopRight => Board[_bottomLeft.X + 1, _bottomLeft.Y + 1];

    public Battenburg(CellReference reference) : base(InitCells(reference), new List<IRule> { new BattenburgRule() })
    {
        _bottomLeft = reference;
    }
    public Battenburg(int x, int y) : this((x,y)) 
    { 

    }

    private static List<CellReference> InitCells(CellReference cellReference)
    {
        List<CellReference> allCells = new List<CellReference>
        {
            cellReference,
            new CellReference(cellReference.X + 1, cellReference.Y),
            new CellReference(cellReference.X + 1, cellReference.Y + 1),
            new CellReference(cellReference.X, cellReference.Y + 1)
        };
        return allCells;
    }
}
