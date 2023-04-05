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

        GetUnits(node.Child, symbolTable, out cells, out units);

        return new Board(cells, units);
    }

    private static void GetUnits(UnitNode node, SymbolTable symbolTable, out List<Cell> cells, out List<Unit> units)
    {
        cells = new List<Cell>();
        units = new List<Unit>();

        foreach (UnitStatementNode child in node.UnitStatements)
        {
            GetFromUnitStatement(child, symbolTable, out List<Cell> childCells, out List<Unit> childUnits);
            cells.AddRange(childCells);
            units.AddRange(childUnits);
        }

        CellReference[] cellReferences = cells
            .Select(c => new CellReference(c.X, c.Y))
            .ToArray();
        units.Add(new Unit(cellReferences, new List<IRule>()));
    }

    private static void GetFromUnitStatement(UnitStatementNode node, SymbolTable symbolTable, out List<Cell> cells, out List<Unit> units)
    {
        cells = new List<Cell>();
        units = new List<Unit>();

        switch (node)
        {
            case UnitNode unitNode:
                GetUnits(unitNode, symbolTable, out cells, out units);
                break;
            case RulesNode rulesNode:
                break;
            case GivensNode givensNode:
                InsertGivenDigit(givensNode, symbolTable, out cells);
                break;
            case FunctionCallNode functionCallNode:
                GetCellsAndUnitsFromFunction(functionCallNode, symbolTable, out cells, out Unit unit);
                units.Add(unit);
                break;
        }
    }

    private static void InsertGivenDigit(GivensNode node, SymbolTable symbolTable, out List<Cell> cells)
    {
        cells = new List<Cell>();

        foreach (GivensStatementNode child in node.GivensStatements)
        {
            if (!ExpressionToInt(child.Digit, symbolTable, out int digit))
            {
                throw new Exception("Givens must contain one, and only one digit.");
            }
            List<Cell> newCells = ExpressionNodeToCells(child.Cell.X, child.Cell.Y, symbolTable, digit);
            if (newCells.Count != 1)
            {
                throw new Exception("Givens must contain one, and only one cell.");
            }
            cells.Add(newCells[0]);
        }
    }

    private static void GetCellsAndUnitsFromFunction(FunctionCallNode node, SymbolTable symbolTable,  out List<Cell> cells, out Unit units)
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

                cells.AddRange(ExpressionNodeToCells(cellNode.X, cellNode.Y, symbolTable));
            }

            CellReference[] cellReferences = cells
                .Select(c => new CellReference(c.X, c.Y))
                .ToArray();
            units = new Unit(cellReferences, new List<IRule>());
        } else {
            throw new ArgumentException($"Function {node.Name.Match} is not supported.");
        }
    }

    private static List<Cell> ExpressionNodeToCells(ExpressionNode nodeX, ExpressionNode nodeY, SymbolTable symbolTable, int digit = Cell.EmptyDigit)
    {
        List<int> xs = ExpressionToInts(nodeX, symbolTable);
        List<int> ys = ExpressionToInts(nodeY, symbolTable);

        List<Cell> cells;
        if (digit == Cell.EmptyDigit)
        {
            cells = CombineListsWithFunction(xs, ys, (x, y) => new Cell(x, y));
        }
        else
        {
            cells = CombineListsWithFunction(xs, ys, (x, y) => new Cell(x, y, digit));
        }

        return cells;
    }

    private static bool ExpressionToInt(ExpressionNode node, SymbolTable symbolTable, out int value)
    {
        List<int> values = ExpressionToInts(node, symbolTable);
        if (values.Count != 1)
        {
            value = 0;
            return false;
        }
        value = values[0];
        return true;
    }

    private static List<int> ExpressionToInts(ExpressionNode node, SymbolTable symbolTable)
    {
        switch (node)
        {
            case RangeNode rangeNode:
                return CalculateRange(rangeNode, symbolTable);
            case BinaryNode binaryNode:
                return CalculateBinaryNode(binaryNode, symbolTable);
            case UnaryNode unaryNode:
                return CalculateUnaryNode(unaryNode, symbolTable);
            case IdentifierNode identifierNode:
                return IdentifierRetriever(identifierNode, symbolTable);
            case ValueNode valueNode:
                if(Int32.TryParse(valueNode.ValueToken.Match, out int value))
                {
                    return new List<int>() { value };
                } 
                throw new ArgumentException($"Value {valueNode.ValueToken.Match} is not an integer.");
            default:
                throw new ArgumentException($"Expression type {node.GetType()} is not supported.");
        }
    }

    private static List<int> CalculateRange(RangeNode node, SymbolTable symbolTable)
    {
        if (!ExpressionToInt(node.MinimumExpression, symbolTable, out int min) ||
            !ExpressionToInt(node.MaximumExpression, symbolTable, out int max))
        {
            throw new Exception("Range nodes cannot contain ranges as either the min or the max value.");
        }

        List<int> values = new List<int>();

        int start = node.IsMinInclusive ? min : min + 1;
        int end = node.IsMaxInclusive ? max + 1 : max;

        for (int i = start; i < end; i++)
        {
            values.Add(i);
        }

        return values;
    }

    private static List<int> CalculateBinaryNode(BinaryNode node, SymbolTable symbolTable)
    {
        List<int> left = ExpressionToInts(node.Left, symbolTable);
        List<int> right = ExpressionToInts(node.Right, symbolTable);
        
        switch(node.BinaryType)
        {
            case BinaryType.Plus:
                return CombineListsWithFunction(left, right, (x, y) => x + y);
            case BinaryType.Minus:
                return CombineListsWithFunction(left, right, (x, y) => x - y);
            case BinaryType.Multiply:
                return CombineListsWithFunction(left, right, (x, y) => x * y);
            case BinaryType.Mod:
                return CombineListsWithFunction(left, right, (x, y) => x % y);
            case BinaryType.Power:
                return CombineListsWithFunction(left, right, (x, y) => (int)Math.Pow(x, y));
            default:
                throw new ArgumentException($"Binary operator {node.BinaryType} is not supported.");
        }
    }

    private static List<int> CalculateUnaryNode(UnaryNode node, SymbolTable symbolTable)
    {
        switch(node.UnaryType)
        {
            case UnaryType.Plus:
                return ExpressionToInts(node.Expression, symbolTable);
            case UnaryType.Minus:
                return ExpressionToInts(node.Expression, symbolTable).Select(i => -i).ToList();
            default:
                throw new ArgumentException($"Unary operator {node.UnaryType} is not supported.");
        }
    }

    private static List<int> IdentifierRetriever(IdentifierNode node, SymbolTable symbolTable)
    {
        throw new NotImplementedException();
    }

    private static List<T> CombineListsWithFunction<T>(List<int> left, List<int> right, Func<int, int, T> func)
    {
        HashSet<T> values = new HashSet<T>(left.Count * right.Count);
        for (int i = 0; i < left.Count; i++)
        {
            for (int j = 0; j < right.Count; j++)
            {
                T value = func.Invoke(left[i], right[j]);
                if (!values.Contains(value))
                {
                    values.Add(value);
                }
            }
        }
        return values.ToList();
    }
}
