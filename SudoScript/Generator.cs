using SudoScript.Ast;
using SudoScript.Data;
using System.Reflection.Metadata.Ecma335;

namespace SudoScript;

public static class Generator
{
    static Generator()
    {
        Plugins.AddUnitFunction("Union", Union);
    }

    private static IEnumerable<Unit> Union(SymbolTable table, object[] args)
    {
        Unit unit = new Unit();
        foreach (object arg in args)
        {
            if (arg is CellReference cell)
            {
                unit.AddCell(cell);
            }
            else
            {
                throw new Exception("Union only accepts Cells as arguments.");
            }
        }
        yield return unit;
    }

    // Takes the program node as input, and creates the initial symboltable,
    // and returns the Board with cells and units
    public static Board GetBoardFromAST(ProgramNode node)
    {
        SymbolTable symbolTable = new SymbolTable();
        return GenerateBoard(node, symbolTable!);
    }

    // Generates the Board using lists of cells and units
    public static Board GenerateBoard(ProgramNode node, SymbolTable symbolTable)
    {
        List<Unit> units = GetUnitFromStatements(node.Child, symbolTable).ToList();

        return new Board(symbolTable.GetCells(), units);
    }

    // Takes the primary node, and travels its children to find units
    private static void AddUnitFunction(UnitNode node, SymbolTable symbolTable)
    {
        Plugins.AddUnitFunction(node.NameToken!.Match, (symbolTable, args) =>
        {
            SymbolTable table = new SymbolTable(symbolTable);
            for (int i = 0; i < args.Length; i++)
            {
                object arg = args[i];
                ParameterNode param = node.Parameters[i];
                if (arg is CellReference cell && param is ParameterCellNode paramCell)
                {
                    table.AddDigit(paramCell.X.NameToken.Match, cell.X);
                    table.AddDigit(paramCell.Y.NameToken.Match, cell.Y);
                }
                else if (arg is int digit && param is ParameterIdentifierNode paramDigit)
                {
                    table.AddDigit(paramDigit.NameToken.Match, digit);
                }
                else
                {
                    throw new Exception($"Invalid parameter type.");
                }
            }
            return GetUnitFromStatements(node, symbolTable);
        });
    }

    private static IEnumerable<Unit> GetUnitFromStatements(UnitNode node, SymbolTable symbolTable)
    {
        Unit unit = new Unit();

        foreach (UnitStatementNode child in node.UnitStatements)
        {
            switch (child)
            {
                case UnitNode unitNode:                 
                    // In case of a UnitNode, it recursively calls the GetUnits function
                    if(unitNode.NameToken is not null)
                    {
                        AddUnitFunction(unitNode, symbolTable);
                    }
                    else
                    {
                        IEnumerable<Unit> units = GetUnitFromStatements(node, symbolTable);
                        foreach (Unit newUnit in units)
                        {
                            yield return newUnit;
                        }
                    }
                    break;
                case RulesNode rulesNode:               
                    // A RulesNode is handled by the RulesHandler function
                    unit.AddRule(GetRules(rulesNode, symbolTable));
                    break;
                case GivensNode givensNode:             
                    // Since a given is a cell with a digit, a function is made to attach a digit to the cell
                    GetGivens(givensNode, symbolTable);
                    break;
                case FunctionCallNode functionCallNode: 
                    // Lastly, a function is used to extract cells and units from functioncalls
                    foreach (Unit newUnit in GetCellsAndUnitsFromFunction(functionCallNode, symbolTable))
                    {
                        unit.AddUnit(newUnit);
                        yield return newUnit;
                    }
                    break;
            }
        }

        yield return unit;
    }

    private static IEnumerable<IRule> GetRules(RulesNode node, SymbolTable symbolTable)
    {
        // Rules will be handled like ranges, so [1;3] will output 3 different lists of arguments

        foreach (FunctionCallNode child in node.Children())
        {
            List<List<object>> argumentCombinations = ArgumentToArgumentCombinations(child.Arguments, symbolTable);

            foreach(List<object> arguments in argumentCombinations)
            {
                yield return Plugins.CreateRule(child.Name.Match, arguments.ToArray());
            }
        }
    }

    private static List<List<object>> ArgumentToArgumentCombinations(IReadOnlyList<ArgumentNode> argumentNodes, SymbolTable symbolTable)
    {
        List<List<object>> argumentCombinations = new List<List<object>>();

        // If there are no arguments, instead call function with only one argument.
        if (argumentNodes.Count == 0)
        {
            argumentCombinations.Add(new List<object>());
        }

        foreach (ArgumentNode argument in argumentNodes)
        {
            List<object> arguments = new List<object>();
            if (argument is CellNode cellNode)
            {
                arguments = ExpressionToCells(cellNode.X, cellNode.Y, symbolTable).Cast<object>().ToList();
            }
            else if (argument is ExpressionNode expressionNode)
            {
                arguments = ExpressionToInts(expressionNode, symbolTable).Cast<object>().ToList();
            }
            else
            {
                throw new NotSupportedException("Arguments must be either expressions or cells.");
            }

            if (argumentCombinations.Count == 0)
            {
                argumentCombinations.Add(arguments);
                continue;
            }

            List<List<object>> oldArgumentCombinations = argumentCombinations;
            argumentCombinations = new List<List<object>>();
            foreach (List<object> oldArguments in oldArgumentCombinations)
            {
                List<List<object>> newArguments = CombineListsWithFunction(oldArguments, arguments, (l, r) => new List<object>() { l, r });
                argumentCombinations.AddRange(newArguments);
            }
        }

        return argumentCombinations;
    }

    // A function to retrieve cells and their given digits, from the AST
    private static void GetGivens(GivensNode node, SymbolTable symbolTable)
    {
        foreach (GivensStatementNode child in node.GivensStatements)
        {
            // Check if the digit is a usable value
            if (!ExpressionToInt(child.Digit, symbolTable, out int digit))
            {
                throw new Exception("Givens must contain one, and only one digit.");
            }

            List<Cell> newCells = ExpressionToCells(child.Cell.X, child.Cell.Y, symbolTable)
                .Select(c => new Cell(c.X, c.Y, digit))
                .ToList();

            // Check if there only exists one cell
            if (newCells.Count != 1)
            {
                throw new Exception("Givens must contain one, and only one cell.");
            }
            
            Cell cell = newCells[0];
            CellReference reference = (cell.X, cell.Y);

            symbolTable.AddCell(reference, cell);
        }
    }
    
    // Extract list of cells and units
    private static IEnumerable<Unit> GetCellsAndUnitsFromFunction(FunctionCallNode node, SymbolTable symbolTable)
    {
        List<List<object>> argumentCombinations = ArgumentToArgumentCombinations(node.Arguments, symbolTable);
        foreach (List<object> arguments in argumentCombinations)
        {
            IEnumerable<Unit> units = Plugins.CreateUnit(node.Name.Match, symbolTable, arguments.ToArray());
            foreach (Unit unit in units)
            {
                foreach (CellReference reference in unit.References())
                {
                    symbolTable.AddCell(reference, new Cell(reference.X, reference.Y));
                }
                yield return unit;
            }
        }
    }

    //Converts ExpressionNode objects into list of integers.
    //Then creates a list of cell using integer list to create a digit parameter.
    private static List<CellReference> ExpressionToCells(ExpressionNode nodeX, ExpressionNode nodeY, SymbolTable symbolTable)
    {
        List<int> xs = ExpressionToInts(nodeX, symbolTable);
        List<int> ys = ExpressionToInts(nodeY, symbolTable);

        List<CellReference> cells;
        cells = CombineListsWithFunction(xs, ys, (x, y) => new CellReference(x, y));
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
    //Method takes UnaryNode and symbolTable and returns a list of ints.
    private static List<int> CalculateUnaryNode(UnaryNode node, SymbolTable symbolTable)
    {
        switch(node.UnaryType)
        {
            case UnaryType.Plus:
                return ExpressionToInts(node.Expression, symbolTable);
                //If the UnaryType is minus, convert the expression to a list of ints and then negate each element.
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

    //Method takes in two lists of T2 and a function for two arguments to return T1 value.
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
