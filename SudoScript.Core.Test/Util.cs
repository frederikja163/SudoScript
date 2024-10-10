﻿using StandardLibrary;
using SudoScript.Core.Data;

namespace SudoScript.Core.Test;

public static class Util
{
    private static readonly List<IRule> _oneRuleRules = new List<IRule>() { new OneRule() };

    public static Unit CreateRow(int y)
    {
        List<CellReference> cells = new List<CellReference>();
        for (int x = 1; x <= 9; x++)
        {
            cells.Add(new CellReference(x, y));
        }
        return new Unit(cells, _oneRuleRules);
    }

    public static Unit CreateColumn(int x)
    {
        List<CellReference> cells = new List<CellReference>();
        for (int y = 1; y <= 9; y++)
        {
            cells.Add(new CellReference(x, y));
        }
        return new Unit(cells, _oneRuleRules);
    }

    public static Unit CreateBox(int x, int y)
    {
        List<CellReference> cells = new List<CellReference>();
        for (int x1 = 0; x1 < 3; x1++)
        {
            for (int y1 = 0; y1 < 3; y1++)
            {
                cells.Add(new CellReference(x + x1, y + y1));
            }
        }
        return new Unit(cells, _oneRuleRules);
    }

    public static Board CreateStandardEmpty(params Unit[] units)
    {
        List<Cell> allCells = new List<Cell>();
        List<Unit> allUnits = new List<Unit>();
        allUnits.AddRange(units);

        for (int x = 1; x <= 9; x++)
        {
            for (int y = 1; y <= 9; y++)
            {
                allCells.Add(new Cell(x, y));
            }
        }
        allUnits.Add(CreateRow(1));
        allUnits.Add(CreateRow(2));
        allUnits.Add(CreateRow(3));
        allUnits.Add(CreateRow(4));
        allUnits.Add(CreateRow(5));
        allUnits.Add(CreateRow(6));
        allUnits.Add(CreateRow(7));
        allUnits.Add(CreateRow(8));
        allUnits.Add(CreateRow(9));

        allUnits.Add(CreateColumn(1));
        allUnits.Add(CreateColumn(2));
        allUnits.Add(CreateColumn(3));
        allUnits.Add(CreateColumn(4));
        allUnits.Add(CreateColumn(5));
        allUnits.Add(CreateColumn(6));
        allUnits.Add(CreateColumn(7));
        allUnits.Add(CreateColumn(8));
        allUnits.Add(CreateColumn(9));

        allUnits.Add(CreateBox(1, 1));
        allUnits.Add(CreateBox(1, 4));
        allUnits.Add(CreateBox(1, 7));
        allUnits.Add(CreateBox(4, 1));
        allUnits.Add(CreateBox(4, 4));
        allUnits.Add(CreateBox(4, 7));
        allUnits.Add(CreateBox(7, 1));
        allUnits.Add(CreateBox(7, 4));
        allUnits.Add(CreateBox(7, 7));

        return new Board(allCells, allUnits);
    }

    public static Board CreateEasyBoard()
    {
        Board board = Util.CreateStandardEmpty();
        // Sudoku givens generated by https://sudoku.com/
        board[1, 1].Digit = 1;
        board[4, 1].Digit = 9;
        board[5, 1].Digit = 6;
        board[8, 1].Digit = 4;

        board[2, 2].Digit = 4;
        board[3, 2].Digit = 3;
        board[8, 2].Digit = 2;

        board[5, 3].Digit = 3;
        board[6, 3].Digit = 7;
        board[7, 3].Digit = 1;

        board[2, 4].Digit = 1;
        board[6, 4].Digit = 4;
        board[7, 4].Digit = 7;
        board[8, 4].Digit = 8;

        board[3, 5].Digit = 4;
        board[4, 7].Digit = 2;

        board[3, 6].Digit = 6;
        board[5, 6].Digit = 7;
        board[7, 6].Digit = 4;
        board[8, 6].Digit = 5;

        board[6, 7].Digit = 1;
        board[7, 7].Digit = 8;
        board[8, 7].Digit = 3;
        board[9, 7].Digit = 5;

        board[3, 9].Digit = 1;
        board[2, 8].Digit = 9;
        board[4, 8].Digit = 7;
        board[5, 8].Digit = 4;

        board[5, 9].Digit = 5;
        board[6, 9].Digit = 8;
        board[7, 9].Digit = 9;
        board[8, 9].Digit = 7;

        return board;
    }

    public static Board CreateSmallBoard()
    {
        List<Cell> allCells = new List<Cell>();
        List<Unit> allUnits = new List<Unit>();

        for (int x = 1; x <= 6; x++)
        {
            for (int y = 1; y <= 6; y++)
            {
                allCells.Add(new Cell(x, y));
            }
        }

        allUnits.Add(CreateBox(1, 1));
        allUnits.Add(CreateBox(1, 4));
        allUnits.Add(CreateBox(4, 4));
        allUnits.Add(CreateBox(4, 1));

        for (int y = 1; y <= 6; y++)
        {
            List<CellReference> cellList = new List<CellReference>();
            for (int x = 1; x <= 6; x++)
            {
                cellList.Add(new CellReference(x, y));
            }
            allUnits.Add(new Unit(cellList, new List<IRule> { new Unique() }));
        }

        for (int x = 1; x <= 6; x++)
        {
            List<CellReference> cellList = new List<CellReference>();
            for (int y = 1; y <= 6; y++)
            {
                cellList.Add(new CellReference(x, y));
            }
            allUnits.Add(new Unit(cellList, new List<IRule> { new Unique() }));
        }

        return new Board(allCells, allUnits);
    }
    
    public static Board CreateWildBoard()
    {
        List<Cell> allCells = new List<Cell>();
        List<Unit> allUnits = new List<Unit>();

        for (int x = 1; x <= 9; x++)
        {
            for (int y = 1; y <= 9; y++)
            {
                allCells.Add(new Cell(x, y));
            }
        }
        for (int x = 10; x <= 12; x++)
        {
            for (int y = 4; y <= 15; y++)
            {
                allCells.Add(new Cell(x, y));
            }
        }

        for (int i = 1; i <= 9; i++)
        {
            allUnits.Add(CreateRow(i));
            allUnits.Add(CreateColumn(i));
        }
        allUnits.Add(CreateBox(1, 1));
        allUnits.Add(CreateBox(1, 4));
        allUnits.Add(CreateBox(1, 7));
        allUnits.Add(CreateBox(4, 1));
        allUnits.Add(CreateBox(4, 4));
        allUnits.Add(CreateBox(4, 7));
        allUnits.Add(CreateBox(7, 1));
        allUnits.Add(CreateBox(7, 4));
        allUnits.Add(CreateBox(7, 7));

        allUnits.Add(CreateBox(10, 7));
        allUnits.Add(CreateBox(10, 10));
        allUnits.Add(CreateBox(10, 13));

        allUnits.Add(new Unit (
            new List<CellReference> {
                new CellReference(4, 4),
                new CellReference(5, 5),
                new CellReference(6, 6),
                new CellReference(7, 7),
                new CellReference(8, 8),
                new CellReference(9, 9),
                new CellReference(10, 10),
                new CellReference(11, 11),
                new CellReference(12, 12)}, 
            new List<IRule> {new OneRule() } ));

        for (int x = 10; x <= 12; x++)
        {
            allUnits.Add(new Unit(
            new List<CellReference> {
                new CellReference(x, 4),
                new CellReference(x, 6),
                new CellReference(x, 5),
                new CellReference(x, 7),
                new CellReference(x, 8),
                new CellReference(x, 9),
                new CellReference(x, 10),
                new CellReference(x, 11),
                new CellReference(x, 12)
                },
            new List<IRule> { new Unique() }));
        }

        allUnits.Add(new Unit(
            new List<CellReference> {
                new CellReference(4, 4),
                new CellReference(4, 5),
                new CellReference(4, 6),
                new CellReference(4, 7),
                new CellReference(6, 1),
                new CellReference(5, 1),
                new CellReference(4, 8),
                new CellReference(8, 3),
                new CellReference(2, 2)},
            new List<IRule> { new AntiKnight() }));

        allUnits.Add(new Unit(
            new List<CellReference> {
                new CellReference(2, 4),
                new CellReference(2, 5),
                new CellReference(9, 6),
                new CellReference(1, 7)},
            new List<IRule> { new AntiKnight() }));

        allUnits.Add(new Unit(
            new List<CellReference> {
                new CellReference(2, 4),
                new CellReference(11, 10),
                new CellReference(6, 9),
                new CellReference(7, 2)},
            new List<IRule> { new Even() }));

        allUnits.Add(new Unit(
            new List<CellReference> {
                new CellReference(4, 8),
                new CellReference(5, 9),
                new CellReference(10, 11)},
            new List<IRule> { new Unique(),
                              new AntiKing()}));

        allUnits.Add(new Unit(
            new List<CellReference> {
                new CellReference(7, 1),
                new CellReference(2, 1),
                new CellReference(1, 6)},
            new List<IRule> { new AntiKing() }));

        Board board = new Board(allCells, allUnits);

        board[3, 9].Digit = 7;
        board[9, 9].Digit = 1;
        board[11, 10].Digit = 6;

        return board;
    }
}
