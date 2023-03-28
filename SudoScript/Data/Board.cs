namespace SudoScript.Data;

public sealed class Board : ICloneable
{
    private IReadOnlyDictionary<(int, int), Cell> _cells;

    private Board(IReadOnlyDictionary<(int, int), Cell> cells, List<Unit> units)
    {
        _cells = cells;
        Units = units;
        foreach (Unit unit in units)
        {
            unit.Board = this;
        }
    }

    public Board(IReadOnlyList<Cell> cells, List<Unit> units)
        : this(cells.ToDictionary(c => (c.X, c.Y)), units)
    {
    }

    public List<Unit> Units { get; }

    public Cell this[int x, int y] => _cells[(x, y)];

    public IEnumerable<Cell> Cells()
    {
        return _cells.Values;
    }

    public void EliminateCandidates()
    {
        foreach (Unit unit in Units)
        {
            unit.EliminateCandidates();
        }
    }

    public bool ValidateRules()
    {
        foreach (Unit unit in Units)
        {
            if (unit.ValidateRules())
            {
                return false;
            }
        }
        return true;
    }

    public Board Clone()
    {
        IReadOnlyDictionary<(int, int), Cell> cellsCopy = _cells.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Board board = new Board(cellsCopy, new List<Unit>());
        foreach (Unit unit in Units)
        {
            Unit unitCopy = new Unit(unit);
            unitCopy.Board = this;
        }
        return board;
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
}
