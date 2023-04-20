using SudoScript.Ast;
using SudoScript.Data;

namespace SudoScript;

public sealed class SymbolTable 
{
    private Dictionary<string, int> _table;

    public SymbolTable(SymbolTable table)
    {
        _table = table._table.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public void Add(string identifier, int value)
    {
        if (_table.ContainsKey(identifier))
        {
            throw new Exception("Duplicate definition of identifier " +  identifier);
        }
        _table.Add(identifier, value);
    }
}