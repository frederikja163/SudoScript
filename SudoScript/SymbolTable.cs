using SudoScript.Ast;
using SudoScript.Data;

namespace SudoScript;

public sealed class SymbolTable 
{
    Dictionary<string, UnitNode> _units = new Dictionary<string, UnitNode>();

    public SymbolTable(ProgramNode node)
    {

    }

    public static void ProcessNode(IAstNode node) 
    {
        switch(node){
            case UnitNode:
                
                break;
            case CellNode:

                break;
            case RulesNode:

                break;
            case GivensNode:

                break;
        }

        foreach (IAstNode child in node.Children())
        {
            ProcessNode(child);
        }
    }
}

public sealed class Symbol
{
    
}