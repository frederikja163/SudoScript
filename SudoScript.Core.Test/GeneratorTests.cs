using NUnit.Framework;
using SudoScript.Core.Ast;
using SudoScript.Core;
using SudoScript.Core.Data;

namespace SudoScript.Core.Test;

internal sealed class GeneratorTests
{
    private static Token Token(string match = "")
    {
        return new Token((TokenType)(-1), match, "", 0, 0, "");
    }

    private static Token Token(TokenType token, string match = "")
    {
        return new Token(token, match, "", 0, 0, "");
    }

    private static ProgramNode Program(UnitNode node)
    {
        return new ProgramNode(node);
    }

    private static UnitNode Unit(params UnitStatementNode[] statements)
    {
        return new UnitNode(null, statements.ToList(), new List<ParameterNode>());
    }

    private static UnitNode Unit(string name, List<ParameterNode> parameter, List<UnitStatementNode> statements)
    {
        return new UnitNode(Token(name), statements, parameter);
    }

    private static List<UnitStatementNode> Statements(params UnitStatementNode[] statements)
    {
        return statements.ToList();
    }

    private static GivensNode Givens(params GivensStatementNode[] givens)
    {
        return new GivensNode(givens.ToList());
    }

    private static GivensStatementNode Given(CellNode cell, ExpressionNode digit)
    {
        return new GivensStatementNode(cell, digit);
    }

    private static RulesNode Rules(params FunctionCallNode[] functions)
    {
        return new RulesNode(Token(), Token(), Token(), functions.ToList());
    }

    private static FunctionCallNode Function(string name, params ArgumentNode[] arguments)
    {
        return new FunctionCallNode(Token(name), arguments.ToList());
    }

    private static List<ParameterNode> Parameters(params ParameterNode[] parameters)
    {
        return parameters.ToList();
    }

    private static ParameterCellNode ParamCell(string left, string right)
    {
        return new ParameterCellNode(ParamIdentifier(left), ParamIdentifier(right));
    }

    private static ParameterIdentifierNode ParamIdentifier(string name)
    {
        return new ParameterIdentifierNode(Token(name));
    }

    private static CellNode Cell(ExpressionNode x, ExpressionNode y)
    {
        return new CellNode(Token(), Token(), Token(), x, y);
    }

    private static CellNode Cell(int x, int y)
    {
        return new CellNode(Token(), Token(), Token(), Value(x), Value(y));
    }

    private static RangeNode Range(ExpressionNode min, ExpressionNode max, bool isMinInclusive, bool isMaxInclusive)
    {
        return new RangeNode(Token(TokenType.LeftBracket), 
            Token(TokenType.RightBracket), Token(TokenType.Semicolon), 
            min, max, isMinInclusive, isMaxInclusive); 
    }
    
    private static ValueNode Value(int digit)
    {
        return new ValueNode(Token(TokenType.Number, $"{digit}"));
    }
    
    private static BinaryNode Binary(ExpressionNode left, BinaryType type, ExpressionNode right)
    {
        return new BinaryNode(Token(), type, left, right);
    }

    private static IdentifierNode Identifier(string name)
    {
        return new IdentifierNode(Token(name));
    }

    private static UnaryNode Unary(UnaryType unaryType, ExpressionNode expression)
    {
        return new UnaryNode(Token(), unaryType, expression);
    }

    [Test]
    public void UnionTest()
    {
        /*
         * (1, 1) (2, 2)
         */

        ProgramNode program = Program(
            Unit(
                Function("Union", Cell(1,1), Cell(2, 2))
                )
        );

        Board board = Generator.GetBoardFromAST(program);

        Assert.That(board.Units.Count, Is.EqualTo(2));
        Assert.That(board.Units.All(u => u.Cells().Count() == 2), Is.True);
    }

    [Test]
    public void UnitWithMultipleCellsTest()
    {
        /* 
         * (1,1)
         * (2,2)
         */

        ProgramNode program = Program(
            Unit(
                Function("Union", Cell(1, 1)),
                Function("Union", Cell(2, 2))
            )
        );

        Board board = Generator.GetBoardFromAST(program);

        Assert.That(board.Units.Count, Is.EqualTo(3));

        Unit[] units = board.Units.OrderBy(u => u.Cells().Count()).ToArray();
        Assert.That(units[0].Cells().Count(), Is.EqualTo(1));
        Assert.That(units[1].Cells().Count(), Is.EqualTo(1));
        Assert.That(units[2].Cells().Count(), Is.EqualTo(2));
    }

    [TestCase(BinaryType.Plus, 5)]
    [TestCase(BinaryType.Minus, 1)]
    [TestCase(BinaryType.Multiply, 6)]
    [TestCase(BinaryType.Mod, 1)]
    [TestCase(BinaryType.Power, 9)]
    public void BinaryOperatorTest(BinaryType type, int expected)
    {
        /* 
         * (3{BinaryType}2,1)
         */

        ProgramNode program = Program(
            Unit(
                Function("Union", Cell(
                        Binary(Value(3), type, Value(2)), Value(1)
                    )
                )
            )
        );

        Board board = Generator.GetBoardFromAST(program);

        Cell cell = board.Cells().First();
        Assert.That(cell.X, Is.EqualTo(expected));
    }

    [TestCase(UnaryType.Minus, -2)]
    [TestCase(UnaryType.Plus, 2)]
    public void UnaryOperatorTest(UnaryType type, int expected)
    {
        /* 
         * ({UnaryType}2,1)
         */

        ProgramNode program = Program(
            Unit(
                Function("Union", Cell(
                        Unary(type, Value(2)), Value(1)
                    )
                )
            )
        );

        Board board = Generator.GetBoardFromAST(program);

        Cell cell = board.Cells().First();
        Assert.That(cell.X, Is.EqualTo(expected));
    }

    [TestCase(false, false, 1)]
    [TestCase(false, true, 2)]
    [TestCase(true, false, 2)]
    [TestCase(true, true, 3)]
    public void RangeInclusivityTest(bool isMinInclusive, bool isMaxInclusive, int expected)
    {
        /* 
         * ([1;3],1)
         */

        ProgramNode program = Program(
            Unit(
                Function("Union", Cell(
                        Range(Value(1), Value(3), isMinInclusive, isMaxInclusive), Value(1)
                    )
                )
            )
        );

        Board board = Generator.GetBoardFromAST(program);
        Assert.That(board.Cells().Count(), Is.EqualTo(expected));
    }

    [Test]
    public void RangeTest()
    {
        /* 
         * ([1;3],1)
         */
        ProgramNode program = Program(
            Unit(
                Function("Union", Cell(
                        Range(Value(1), Value(3), true, true), Value(1)
                    )
                )
            )
        );

        Board board = Generator.GetBoardFromAST(program);
        Assert.DoesNotThrow(() => { Cell c = board[1, 1]; });
        Assert.DoesNotThrow(() => { Cell c = board[2, 1]; });
        Assert.DoesNotThrow(() => { Cell c = board[3, 1]; });
    }

    [Test]
    public void GivenTest()
    {
        /*
         * Givens {
         *  (1,1) 2
         *  (2,2) 9
         * }
         */
        ProgramNode program = Program(
                Unit(
                        Givens(Given(Cell(1,1), Value(2))),
                        Givens(Given(Cell(2,2), Value(9)))
                    )
                );

        Board board = Generator.GetBoardFromAST(program);
        Assert.That(board[1, 1].IsGiven, Is.True);
        Assert.That(board[1, 1].Digit, Is.EqualTo(2));
        Assert.That(board[1, 1].CandidateCount, Is.EqualTo(1));

        Assert.That(board[2, 2].IsGiven, Is.True);
        Assert.That(board[2, 2].Digit, Is.EqualTo(9));
        Assert.That(board[2, 2].CandidateCount, Is.EqualTo(1));
    }

    [Test]
    public void RulesNoParametersTest()
    {
        /*
         * Rules {
         *  TestRule1
         * }
         */
        ProgramNode program = Program(
                Unit(
                    Rules(Function(nameof(TestRule1)))
                    )
                );

        Board board = Generator.GetBoardFromAST(program);
        Assert.That(board.Units.First().Rules().Count(), Is.EqualTo(1));
        Assert.IsInstanceOf<TestRule1>(board.Units.First().Rules().First());
    }

    [Test]
    public void RulesWithParametersTest()
    {
        /*
         * Rules {
         *  TestRule2 (1,2) 3
         * }
         */
        ProgramNode program = Program(
                Unit(
                    Rules(Function(nameof(TestRule2), Cell(1, 2), Value(3)))
                    )
                );

        Board board = Generator.GetBoardFromAST(program);
        Assert.That(board.Units.First().Rules().Count(), Is.EqualTo(1));
        Assert.IsInstanceOf<TestRule2>(board.Units.First().Rules().First());
        TestRule2 rule = (TestRule2)board.Units.First().Rules().First();
        Assert.That(rule.Cell.X, Is.EqualTo(1));
        Assert.That(rule.Cell.Y, Is.EqualTo(2));
        Assert.That(rule.Digit, Is.EqualTo(3));
    }

    [Test]
    public void UnitFunctionsNoParametersFromPluginsTest()
    {
        /*
         * UnitTest1
         */
        ProgramNode program = Program(
                Unit(
                    Function(nameof(TestUnit1))
                    )
                );

        Board board = Generator.GetBoardFromAST(program);
        Assert.IsInstanceOf<TestUnit1>(board.Units.First());
    }

    [Test]
    public void UnitFunctionsWithParametersFromPluginsTest()
    {
        /*
         * UnitTest2 (4,5) 6
         */
        ProgramNode program = Program(
                Unit(
                    Function(nameof(TestUnit2), Cell(4, 5), Value(6))
                    )
                );

        Board board = Generator.GetBoardFromAST(program);
        Assert.IsInstanceOf<TestUnit2>(board.Units.First());
        TestUnit2 unit = (TestUnit2)board.Units.First();
        Assert.That(unit.Cell.X, Is.EqualTo(4));
        Assert.That(unit.Cell.Y, Is.EqualTo(5));
        Assert.That(unit.Digit, Is.EqualTo(6));
    }
}