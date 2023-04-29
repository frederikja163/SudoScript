using System.Runtime.CompilerServices;
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
    }

    public Board(IReadOnlyList<Cell> cells, IReadOnlyList<Unit> units)
        : this(cells.ToDictionary(c => (c.X, c.Y)), units)
    {
    }

    public IReadOnlyList<Unit> Units { get; }

    public Cell this[int x, int y] => _cells[(x, y)];

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
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (Cell cell in _cells.Values)
        {
            if (cell.X > maxX)
                maxX = cell.X;
            if (cell.X < minX)
                minX = cell.X;
            if (cell.Y > maxY)
                maxY = cell.Y;
            if (cell.Y < minY)
                minY = cell.Y;
        }

        StringBuilder s = new StringBuilder();
        for (int row = maxX; row >= minX; row--)
        {
            for (int col = minX; col <= maxX; col++)
            {
                s.Append(_cells.TryGetValue((row, col), out Cell? cell) ? cell.Digit : '.');
            }
            s.AppendLine();
        }

        return s.ToString();
    }
}
