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
        Dictionary<CellReference, Cell> cells = new Dictionary<CellReference, Cell>();
        List<Unit> units = new List<Unit>();

        GetUnits(node.Child, symbolTable, cells, units);

        return new Board(cells.Values.ToList(), units);
    }

    // Takes the primary node, and travles its children to find units
    private static void GetUnits(UnitNode node, SymbolTable symbolTable, Dictionary<CellReference, Cell> cells, List<Unit> units)
    {
        Unit unit = new Unit();

        foreach (UnitStatementNode child in node.UnitStatements)
        {
            switch (child)
            {
                case UnitNode unitNode:                 // In case of a UnitNode, it recursively calls the GetUnits function
                    GetUnits(unitNode, symbolTable, cells, units);
                    break;
                case RulesNode rulesNode:               // A RulesNode is handled by the RulesHandler function
                    GetRules(rulesNode, symbolTable, unit);
                    break;
                case GivensNode givensNode:             // Since a given is a cell with a digit, a function is made to attach a digit to the cell
                    InsertGivenDigit(givensNode, symbolTable, cells);
                    break;
                case FunctionCallNode functionCallNode: // Lastly, a function is used to extract cells and units from functioncalls
                    Unit newUnit = GetCellsAndUnitsFromFunction(functionCallNode, symbolTable, cells);
                    units.Add(newUnit);
                    unit.AddUnit(newUnit);
                    break;
            }

        }
        
    }

    private static void GetRules(RulesNode node, SymbolTable symbolTable, Unit unit)
    {
        // Rules will be handled like ranges, so [1;3] will output 3 different lists of arguments

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
                IRule rule = Plugins.CreateRule(child.Name.Match, arguments.ToArray());
                unit.AddRule(rule);
            }
        }
    }

    // A function to retrieve cells and their given digits, from the AST
    private static void InsertGivenDigit(GivensNode node, SymbolTable symbolTable, Dictionary<CellReference, Cell> cells)
    {
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

            Cell cell = newCells[0];
            CellReference cellReference = (cell.X, cell.Y);

            if (cells.TryGetValue(cellReference, out Cell? existingCell) && existingCell.IsGiven)
            {
                throw new Exception("Cell already contains a given digit");
            }

            cells[cellReference] = cell;
        }
    }
    
    // Extract list of cells and units
    private static Unit GetCellsAndUnitsFromFunction(FunctionCallNode node, SymbolTable symbolTable, Dictionary<CellReference, Cell> cells)
    {
        if(node.Name.Match == "union")
        {
            Unit unit = new Unit();
            foreach (ArgumentNode child in node.Children())
            {
                if(child is not CellNode cellNode)
                {
                    throw new ArgumentException("Union function only accepts cell arguments.");
                }

                List<Cell> newCells = ExpressionToCells(cellNode.X, cellNode.Y, symbolTable);
                foreach (Cell cell in newCells)
                {
                    CellReference cellReference = (cell.X, cell.Y);
                    if (!cells.ContainsKey(cellReference))
                    {
                        cells.Add(cellReference, cell);
                    }

                    unit.AddCell(cellReference);
                }
            }
            return unit;
        } else {
            throw new ArgumentException($"Function {node.Name.Match} is not supported.");
        }
    }

    //Converts ExpressionNode objects into list of integers.
    //Then creates a list of cell using integer list to create a digit parameter.
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
    //Method takes UnaryNode and symbolTable and returns a list of ints.
    private static List<int> CalculateUnaryNode(UnaryNode node, SymbolTable symbolTable)
    {
        switch(node.UnaryType)
        {
            case UnaryType.Plus:
                return ExpressionToInts(node.Expression, symbolTable);
                //If the UnaryType is minus, convert the expression to list 
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
