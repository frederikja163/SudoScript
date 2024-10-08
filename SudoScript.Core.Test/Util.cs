using StandardLibrary;
using SudoScript.Core.Data;

namespace SudoScript.Core.Test;

internal static class Util
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
}
