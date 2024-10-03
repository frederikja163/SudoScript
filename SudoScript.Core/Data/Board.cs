using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SudoScript.Core.Data;

public sealed class Board: ICloneable
{
    private readonly IReadOnlyDictionary<(int, int), Cell> _cells;

    private Board(IReadOnlyDictionary<(int, int), Cell> cells, IReadOnlyList<Unit> units)
    {
        _cells = cells;
        Units = units;
        foreach(Unit unit in units)
        {
            unit.Board = this;
        }

        MaxX = int.MinValue;
        MaxY = int.MinValue;
        MinX = int.MaxValue;
        MinY = int.MaxValue;
        foreach (CellReference cellReference in _cells.Keys)
        {
                if (cellReference.X > MaxX)
                    MaxX = cellReference.X;
                if (cellReference.X < MinX)
                    MinX = cellReference.X;
                if (cellReference.Y > MaxY)
                    MaxY = cellReference.Y;
                if (cellReference.Y < MinY)
                    MinY = cellReference.Y;
        }
    }

    public Board(IReadOnlyList<Cell> cells, IReadOnlyList<Unit> units)
        : this(cells.ToDictionary(c => (c.X, c.Y)), units)
    {
    }
    
    public int MinX { get; }
    public int MaxX { get; }
    public int MinY { get; }
    public int MaxY { get; }

    public IReadOnlyList<Unit> Units { get; }

    public Cell this[int x, int y] => _cells[(x, y)];

    public bool TryGetCell(int x, int y, [NotNullWhen(true)] out Cell? cell)
    {
        return _cells.TryGetValue((x, y), out cell);
    }
    
    public bool TryGetCell(CellReference cellReference, [NotNullWhen(true)] out Cell? cell)
    {
        return _cells.TryGetValue(cellReference, out cell);
    }

    public IEnumerable<Cell> Cells()
    {
        return _cells.Values;
    }

    public bool EliminateCandidates()
    {
        bool somethingEliminated = false;
        foreach (Unit unit in Units)
        {
            if (unit.EliminateCandidates())
            {
                somethingEliminated = true;
            }
        }
        return somethingEliminated;
    }

    public bool ValidateRules()
    {
        foreach(Unit unit in Units)
        {
            if (!unit.ValidateRules())
            {
                return false;
            }
        }

        return true;
    }

    public bool Validate()
    {
        if ((_cells == null) || !this.ValidateRules())
        {
            return false;
        }
        foreach (Cell cell in this.Cells())
        {
            if (cell.CandidateCount < 1)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsSolved()
    {
        return ValidateRules() && Cells().All(c => c.Digit != Cell.EmptyDigit);
    }

    public Board Clone()
    {
        IReadOnlyDictionary<(int, int), Cell> cellsCopy = _cells.ToDictionary(kvp => kvp.Key, kvp => new Cell(kvp.Value));
        List<Unit> unitsCopy = Units.Select(u => new Unit(u)).ToList();
        Board board = new Board(cellsCopy, unitsCopy);
        return board;
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public override string ToString()
    {
        StringBuilder s = new StringBuilder();
        for (int row = MaxY; row >= MinY; row--)
        {
            for (int col = MinX; col <= MaxX; col++)
            {
                s.Append(_cells.TryGetValue((col, row), out Cell? cell) ? cell.ToString("Digit") : ' ');
            }
            s.AppendLine();
        }

        return s.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Board other = (Board)obj;

        for (int i = 1; i <= _cells.Count; i++)
        {
            for (int j = 1; j <= _cells.Count; j++)
            {
                // Try to get cells from both boards
                if (!TryGetCell(i, j, out Cell? cell1) || !other.TryGetCell(i, j, out Cell? cell2))
                {
                    // If either cell retrieval fails, boards are not equal
                    return false;
                }

                // Compare the two cells
                if (!cell1.Equals(cell2))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var cell in _cells.Values)
        {
            hash = hash * 31 + cell.GetHashCode();
        }
        return hash;
    }
}
