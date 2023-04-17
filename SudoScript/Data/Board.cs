using System.Runtime.CompilerServices;

namespace SudoScript.Data;

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
        return ToString();
    }

    public string ToString(int cellSize = 2)
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach(Cell cell in _cells.Values)
        {
            if(cell.X > maxX) maxX = cell.X;
            if(cell.X < minX) minX = cell.X;
            if(cell.Y > maxY) maxY = cell.Y;
            if(cell.Y < minY) minY = cell.Y;
        }

        string s = "";
        string newlines = new('\n', cellSize / 3 + 1);

        for(int row = maxX; row >= minX; row--)
        {
            for(int col = minX; col <= maxX; col++)
            {
                s += Center(VisualizeCellAt(col, row), cellSize);
            }
            s += row == minY ? "" : newlines;
        }

        return s;
    }

    private string VisualizeCellAt(int x, int y)
    {
        if(_cells.TryGetValue((x, y), out Cell? cell))
        {
            return cell.ToString();
        }
        else
        {
            return "";
        }
    }

    private static string Center(string s, int length)
    {
        int left = Math.Abs((int)((length - s.Length) / 2d));
        if(s.Length < length)
        {
            int rightPad = (int)Math.Ceiling((length - s.Length) / 2d);
            return new string(' ', left) + s + new string(' ', rightPad);
        } else
        {
            return s.Substring(left, length);
        }
    }
}
