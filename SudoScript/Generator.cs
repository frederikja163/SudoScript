using SudoScript.Ast;
using SudoScript.Data;

namespace SudoScript;

public static class Generator
{
    // Takes the program node as input, and creates the initial symboltable (WIP),
    // and returns the Board with cells and units
    public static Board GetBoardFromAST(ProgramNode node)
    {
        SymbolTable symbolTable = new SymbolTable(node);
        return GenerateBoard(node, symbolTable);
    }

    // Generates the Board using lists of cells and units
    public static Board GenerateBoard(ProgramNode node, SymbolTable symbolTable)
    {
        List<Cell> cells = new List<Cell>();
        List<Unit> units = new List<Unit>();
        List<IRule> rules = new List<IRule>();

        GetUnits(node.Child, symbolTable, out cells, out units, out rules);

        return new Board(cells, units);
    }

    // Takes the primary node, and travles its children to find units
    private static void GetUnits(UnitNode node, SymbolTable symbolTable, out List<Cell> cells, out List<Unit> units, out List<IRule> rules)
    {
        cells = new List<Cell>();
        units = new List<Unit>();
        rules = new List<IRule>();

        foreach (UnitStatementNode child in node.UnitStatements)
        {
            GetFromUnitStatement(child, symbolTable, out List<Cell> childCells, out List<Unit> childUnits, out List<IRule> childRules);
            cells.AddRange(childCells);
            units.AddRange(childUnits);
            rules.AddRange(childRules);
        }

        CellReference[] cellReferences = cells
            .Select(c => new CellReference(c.X, c.Y))
            .ToArray();
        units.Add(new Unit(cellReferences, new List<IRule>()));
    }

    // Travels the UnitStatementNode nodes, and handles them according to type
    private static void GetFromUnitStatement(UnitStatementNode node, SymbolTable symbolTable, out List<Cell> cells, out List<Unit> units, out List<IRule> rules)
    {
        cells = new List<Cell>();
        units = new List<Unit>();
        rules = new List<IRule>();

        switch (node)
        {
            case UnitNode unitNode:                 // In case of a UnitNode, it recursively calls the GetUnits function
                GetUnits(unitNode, symbolTable, out cells, out units, out rules);
                break;
            case RulesNode rulesNode:               // A RulesNode is handled by the RulesHandler function
                RulesHandler(rulesNode, symbolTable, out rules);
                break;
            case GivensNode givensNode:             // Since a given is a cell with a digit, a function is made to attach a digit to the cell
                InsertGivenDigit(givensNode, symbolTable, out cells);
                break;
            case FunctionCallNode functionCallNode: // Lastly, a function is used to extract cells and units from functioncalls
                GetCellsAndUnitsFromFunction(functionCallNode, symbolTable, out cells, out Unit unit);
                units.Add(unit);
                break;
        }
    }

    private static void RulesHandler(RulesNode node, SymbolTable symbolTable, out List<IRule> rules)
    {
        // Rules will be handled like ranges, so [1;3] will output 3 different lists of arguments
        rules = new List<IRule>();

        foreach (FunctionCallNode child in node.Children())
        {
            // A list containing the integers and cell references as arguments to the rule.
            List<List<object>> argumentCombinations = new List<List<object>>();
            
            foreach (ArgumentNode argument in child.Arguments)
            {
                List<object> arguments = new List<object>();
                if (argument is CellNode cellNode)
                {
                    arguments = ExpressionToCells(cellNode.X, cellNode.Y, symbolTable).Cast<object>().ToList();
                }
                else if (argument is ExpressionNode expressionNode){
                    arguments = ExpressionToInts(expressionNode, symbolTable).Cast<object>().ToList();
                }
                else
                {
                    throw new NotSupportedException("Arguments must be either expressions or cells.");
                }

                List<List<object>> oldArgumentCombinations = argumentCombinations;
                argumentCombinations = new List<List<object>>();
                foreach (List<object> oldArguments in oldArgumentCombinations)
                {
                    List<List<object>> newArguments = CombineListsWithFunction(oldArguments, arguments, (l, r) => new List<object>() { l, r });
                    argumentCombinations.AddRange(newArguments);
                }
            }

            foreach(List<object> arguments in argumentCombinations)
            {
                IRule rule = CreateRule(child.Name.Match, arguments.ToArray());
                rules.Add(rule);
            }
        }
    }

    private static IRule CreateRule(string name, params object[] args)
    {
        throw new NotImplementedException();
    }

    // A function to retrieve cells and their given digits, from the AST
    private static void InsertGivenDigit(GivensNode node, SymbolTable symbolTable, out List<Cell> cells)
    {
        cells = new List<Cell>();

        foreach (GivensStatementNode child in node.GivensStatements)
        {
            // Check if the digit is a usable value
            if (!ExpressionToInt(child.Digit, symbolTable, out int digit))
            {
                throw new Exception("Givens must contain one, and only one digit.");
            }

            List<Cell> newCells = ExpressionToCells(child.Cell.X, child.Cell.Y, symbolTable, digit);

            // Check if there only exists one cell
            if (newCells.Count != 1)
            {
                throw new Exception("Givens must contain one, and only one cell.");
            } 
            cells.Add(newCells[0]);
        }
    }

    private static void GetCellsAndUnitsFromFunction(FunctionCallNode node, SymbolTable symbolTable, out List<Cell> cells, out Unit units)
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

                cells.AddRange(ExpressionToCells(cellNode.X, cellNode.Y, symbolTable));
            }

            CellReference[] cellReferences = cells
                .Select(c => new CellReference(c.X, c.Y))
                .ToArray();
            units = new Unit(cellReferences, new List<IRule>());
        } else {
            throw new ArgumentException($"Function {node.Name.Match} is not supported.");
        }
    }

    private static List<Cell> ExpressionToCells(ExpressionNode nodeX, ExpressionNode nodeY, SymbolTable symbolTable, int digit = Cell.EmptyDigit)
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

    private static List<T1> CombineListsWithFunction<T1, T2>(List<T2> left, List<T2> right, Func<T2, T2, T1> func)
    {
        HashSet<T1> values = new HashSet<T1>(left.Count * right.Count);
        for (int i = 0; i < left.Count; i++)
        {
            for (int j = 0; j < right.Count; j++)
            {
                T1 value = func.Invoke(left[i], right[j]);
                if (!values.Contains(value))
                {
                    values.Add(value);
                }
            }
        }
        return values.ToList();
    }
}
