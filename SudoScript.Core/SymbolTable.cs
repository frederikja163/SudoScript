using SudoScript.Core.Data;

namespace SudoScript.Core;

public sealed class SymbolTable 
{
    private readonly Dictionary<string, int> _digitTable;
    private readonly Dictionary<CellReference, Cell> _cellTable;

    public SymbolTable()
    {
        _digitTable = new Dictionary<string, int>();
        _cellTable = new Dictionary<CellReference, Cell>();
    }

    public SymbolTable(SymbolTable table)
    {
        _digitTable = table._digitTable.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        _cellTable = table._cellTable;
    }

    public void AddDigit(string identifier, int value)
    {
        if (_digitTable.ContainsKey(identifier))
        {
            throw new Exception("Duplicate definition of identifier " +  identifier);
        }
        _digitTable.Add(identifier, value);
    }

    public void AddCell(CellReference reference, Cell cell)
    {
        if (!_cellTable.TryGetValue(reference, out Cell? oldCell))
        {
            _cellTable.Add(reference, cell);
            return;
        }

        if (oldCell.IsGiven && cell.IsGiven)
        {
            throw new Exception("Multiple definitions of the given for cell " + cell.X + " " + cell.Y);
        }
        else if (cell.IsGiven)
        {
            _cellTable[reference] = cell;
        }
    }

    public Cell[] GetCells()
    {
        return _cellTable.Values.ToArray();
    }
}