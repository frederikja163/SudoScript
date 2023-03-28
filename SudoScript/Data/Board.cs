namespace SudoScript.Data;

public sealed class Board : ICloneable
{
    private readonly IReadOnlyDictionary<(int, int), Cell> _cells;

    private Board(IReadOnlyDictionary<(int, int), Cell> cells, IReadOnlyList<Unit> units)
    {
        _cells = cells;
        Units = units;
        foreach (Unit unit in units)
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
        List<Unit> unitsCopy = Units.Select(u => new Unit(u)).ToList();
        Board board = new Board(cellsCopy, unitsCopy);
        return board;
    }

    object ICloneable.Clone()
    {
        return Clone();
    }
}
