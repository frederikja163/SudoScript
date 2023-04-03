using SudoScript.Ast;
using SudoScript.Data;

namespace SudoScript;

public static class Generator
{
    // Implement this using some sort of visitor pattern perhaps?
    // Maybe the generator itself should be a visitor.
    public static Board GetBoardFromAST(ProgramNode node)
    {
        SymbolTable symbolTable = new SymbolTable(node);
        return GenerateBoard(node, symbolTable);
    }

    public static Board GenerateBoard(ProgramNode node, SymbolTable symbolTable)
    {
        List<Cell> cells = new List<Cell>();
        List<Unit> units = new List<Unit>();

        RetrieveUnits(node.Child, symbolTable, out cells, out units);

        return new Board(cells, units);
    }

    private static void RetrieveUnits(UnitNode node, SymbolTable symbolTable, out List<Cell> cells, out List<Unit> units)
    {
        cells = new List<Cell>();
        units = new List<Unit>();

        foreach (UnitStatementNode child in node.UnitStatements)
        {
            RetrieveFromUnitStatement(child, symbolTable, out List<Cell> childCells, out List<Unit> childUnits);
            cells.AddRange(childCells);
            units.AddRange(childUnits);
        }

        CellReference[] cellReferences = cells
            .Select(c => new CellReference(c.X, c.Y))
            .ToArray();
        units.Add(new Unit(cellReferences, new List<IRule>()));
    }

    private static void RetrieveFromUnitStatement(UnitStatementNode node, SymbolTable symbolTable, out List<Cell> cells, out List<Unit> units)
    {
        cells = new List<Cell>();
        units = new List<Unit>();

        switch (node)
        {
            case UnitNode unitNode:
                RetrieveUnits(unitNode, symbolTable, out cells, out units);
                break;
            case RulesNode rulesNode:
                break;
            case GivensNode givensNode:
                InsertGivenDigit(givensNode, symbolTable, out cells);
                break;
            case FunctionCallNode functionCallNode:
                RetrieveFromFunction(functionCallNode, symbolTable, out cells, out Unit unit);
                units.Add(unit);
                break;
        }
    }

    private static void InsertGivenDigit(GivensNode node, SymbolTable symbolTable, out List<Cell> cells)
    {
        cells = new List<Cell>();

        foreach (GivensStatementNode child in node.GivensStatements)
        {
            if (child.Cell is not CellNode cellNode)
            {
                throw new ArgumentException("Givens only accepts cell arguments.");
            }

            int X = ExpressionToInt(cellNode.X, symbolTable);
            int Y = ExpressionToInt(cellNode.Y, symbolTable);
            int Digit = ExpressionToInt(child.Digit, symbolTable);
            Cell cell = new Cell(X, Y, Digit);
            cells.Add(cell);
        }
    }

    private static void RetrieveFromFunction(FunctionCallNode node, SymbolTable symbolTable,  out List<Cell> cells, out Unit units)
    {
        cells = new List<Cell>();
        

        if(node.Name.Match == "union")
        {
            foreach (ArgumentNode child in node.Children())
            {
                if(child is not CellNode cellNode)
                {
                    throw new ArgumentException("Union function only accepts cell arguments.");
                } 

                int X = ExpressionToInt(cellNode.X, symbolTable);
                int Y = ExpressionToInt(cellNode.Y, symbolTable);
                Cell cell = new Cell(X, Y);
                cells.Add(cell);
            }

            CellReference[] cellReferences = cells
                .Select(c => new CellReference(c.X, c.Y))
                .ToArray();
            units = new Unit(cellReferences, new List<IRule>());
        } else {
            throw new ArgumentException($"Function {node.Name.Match} is not supported.");
        }
    }

    private static int ExpressionToInt(ExpressionNode node, SymbolTable symbolTable)
    {

        switch (node)
        {
            case RangeNode rangeNode:
                return RangeHandler(rangeNode, symbolTable);
            case BinaryNode binaryNode:
                return BinaryTypeFinder(binaryNode, symbolTable);
            case UnaryNode unaryNode:
                return UnaryTypeFinder(unaryNode, symbolTable);
            case IdentifierNode identifierNode:
                return IdentifierRetriever(identifierNode, symbolTable);
            case ValueNode valueNode:
                if(Int32.TryParse(valueNode.ValueToken.Match, out int value))
                {
                    return value;
                } 
                throw new ArgumentException($"Value {valueNode.ValueToken.Match} is not an integer.");
            default:
                throw new ArgumentException($"Expression type {node.GetType()} is not supported.");
        }
    }

    private static int RangeHandler(RangeNode node, SymbolTable symbolTable)
    {
        throw new NotImplementedException();
    }

    private static int BinaryTypeFinder(BinaryNode node, SymbolTable symbolTable)
    {
        switch(node.BinaryType)
        {
            case BinaryType.Plus:
                return ExpressionToInt(node.Left, symbolTable) + ExpressionToInt(node.Right, symbolTable);
            case BinaryType.Minus:
                return ExpressionToInt(node.Left, symbolTable) - ExpressionToInt(node.Right, symbolTable);
            case BinaryType.Multiply:
                return ExpressionToInt(node.Left, symbolTable) * ExpressionToInt(node.Right, symbolTable);
            case BinaryType.Mod:
                return ExpressionToInt(node.Left, symbolTable) % ExpressionToInt(node.Right, symbolTable);
            case BinaryType.Power:
                return (int)Math.Pow(ExpressionToInt(node.Left, symbolTable), ExpressionToInt(node.Right, symbolTable));
            default:
                throw new ArgumentException($"Binary operator {node.BinaryType} is not supported.");
        }
    }

    private static int UnaryTypeFinder(UnaryNode node, SymbolTable symbolTable)
    {
        switch(node.UnaryType)
        {
            case UnaryType.Plus:
                return ExpressionToInt(node.Expression, symbolTable);
            case UnaryType.Minus:
                return -ExpressionToInt(node.Expression, symbolTable);
            default:
                throw new ArgumentException($"Unary operator {node.UnaryType} is not supported.");
        }
    }

    private static int IdentifierRetriever(IdentifierNode node, SymbolTable symbolTable)
    {
        throw new NotImplementedException();
    }
}
