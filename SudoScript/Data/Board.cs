﻿namespace SudoScript.Data;

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

    public string Stringify() {

        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach(Cell cell in _cells.Values) {
            if(cell.X > maxX) maxX = cell.X;
            if(cell.X < minX) minX = cell.X;
            if(cell.Y > maxY) maxY = cell.Y;
            if(cell.Y < minY) minY = cell.Y;
        }

        List<Cell> orderedCellList = _cells.Values
            .OrderBy(c => c.X)
            .OrderBy(c => c.Y)
            .ToList();

        string s = "";

        int i = 0;
        int width = maxX - minX;
        int height = maxY - minY;
        for(int row = 0; row < height; row++) {
            for(int col = 0; col < width; col++) {

                string digitString = " ";
                if(orderedCellList[i].X == col + minX && orderedCellList[i].Y == row + minY) {
                    if(orderedCellList[i].Digit != Cell.EmptyDigit) {
                        digitString = orderedCellList[i].Digit.ToString();
                    }
                    i++;
                }

                string space = col == width - 1 ? "\t" : "";
                s += $"[{digitString}]" + space;
            }
            s += row == maxY - minY - 1 ? "\n\n" : "";
        }

        return s;
    }
}
