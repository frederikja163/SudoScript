namespace SudoScript.Core.Data;

public sealed record CellReference(int X, int Y)
{
    public static implicit operator (int X, int Y)(CellReference cellReference) => (cellReference.X, cellReference.Y);
    public static implicit operator CellReference((int X, int Y) cellReference) => new CellReference(cellReference.X, cellReference.Y);

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}
