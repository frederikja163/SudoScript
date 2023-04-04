using SudoScript.Ast;
using SudoScript.Data;

namespace SudoScript;

// Define the types of symbols that can be stored in the symbol table
public enum SymbolType
{
    Unit,
    Parameter,
    UnitStatement,
    FunctionCall,
    Argument,
    Cell,
    Expression,
    Element,
    Range,
    Rules,
    RuleStatement,
    Givens,
    GivensStatement,
    Identifier,
    Number,
    UnaryOperator,
    BinaryOperator,
    LeftParenthesis,
    RightParenthesis,
    LeftBracket,
    RightBracket,
    Comma,
    QuestionMark,
    Space,
    Newline
}

// Define the symbol table class
public sealed class SymbolTable 
{
    private Dictionary<string, List<Symbol>> _table;

    public SymbolTable(ProgramNode node)
    {
        _table = new Dictionary<string, List<Symbol>>();
    }

    // Add a new symbol to the table
    public void Add(string identifier, SymbolType type, string value, int lineNumber)
    {
        if (!_table.ContainsKey(identifier))
        {
            _table[identifier] = new List<Symbol>();
        }

        _table[identifier].Add(new Symbol(type, value, lineNumber));
    }

    // Check if a symbol is in the table
    public bool Contains(string identifier)
    {
        return _table.ContainsKey(identifier);
    }

    // Get all symbols with a given identifier
    public List<Symbol> Get(string identifier)
    {
        if (!_table.ContainsKey(identifier))
        {
            return new List<Symbol>();
        }

        return _table[identifier];
    }
    
    // Get the most recently added symbol with a given identifier
    public Symbol GetLatest(string identifier)
    {
        if (!_table.ContainsKey(identifier))
        {
            throw new Exception($"Symbol {identifier} not found in symbol table");
        }

        return _table[identifier][_table[identifier].Count - 1];
    }
}

// Define a class for storing information about a single symbol in the symbol table
public sealed class Symbol
{
    public SymbolType Type { get; }
    public string Value { get; }
    public int LineNumber { get; }

    public Symbol(SymbolType type, string value, int lineNumber)
    {
        Type = type;
        Value = value;
        LineNumber = lineNumber;
    }
}