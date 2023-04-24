using NUnit.Framework;
using SudoScript.Ast;
using SudoScript;
using System.Linq.Expressions;
using SudoScript.Data;

namespace Tests;

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
         * */

        ProgramNode program = Program(
            Unit(
                Function("Union", Cell(1,1), Cell(2, 2))
                )
        );

        Board board = Generator.GetBoardFromAST(program);

        Assert.That(board.Units.Count, Is.EqualTo(2));
    }

    [Test]
    public void GenerateBoard()
    {
        // Arrange
        var program = new ProgramNode(
            new UnitNode(
                null,
                new List<UnitStatementNode>()
                {
                    new UnitNode(
                        null,
                        new List<UnitStatementNode>()
                        {
                            new GivensNode(
                                new List<GivensStatementNode>()
                                {
                                    new GivensStatementNode( // (9, 5) 6
                                        new CellNode ( // (9, 5)
                                            new Token(0, "", "", 0, 0, ""),
                                            new Token(0, "", "", 0, 0, ""),
                                            new Token(0, "", "", 0, 0, ""),
                                            new BinaryNode(
                                                new Token(TokenType.Plus, "", "", 0, 0, ""),
                                                BinaryType.Plus,
                                                new ValueNode(
                                                    new Token(TokenType.Number, "8", "", 0, 0, "")
                                                ),
                                                new ValueNode(
                                                    new Token(TokenType.Number, "1", "", 0, 0, "")
                                                )
                                            ),
                                            new ValueNode(
                                                new Token(TokenType.Number, "5", "", 0, 0, "")
                                            )
                                        ),
                                        new ValueNode(
                                            new Token(TokenType.Number, "6", "", 0, 0, "")
                                        )
                                    ),
                                    new GivensStatementNode ( // (3, 6) 2
                                        new CellNode ( // (3, 6)
                                            new Token(0, "", "", 0, 0, ""),
                                            new Token(0, "", "", 0, 0, ""),
                                            new Token(0, "", "", 0, 0, ""),
                                            new ValueNode(
                                                new Token(TokenType.Number, "3", "", 0, 0, "")
                                            ),
                                            new ValueNode(
                                                new Token(TokenType.Number, "6", "", 0, 0, "")
                                            )
                                        ),
                                        new ValueNode(
                                            new Token(TokenType.Number, "2", "", 0, 0, "")
                                        )
                                    )
                                }
                            ),
                            new FunctionCallNode(
                                new Token(TokenType.Identifier, "Union", "", 0, 0, ""),
                                new List<ArgumentNode>
                                {
                                    new CellNode ( // ([2 ; 4], 7)
                                        new Token(0, "", "", 0, 0, ""),
                                        new Token(0, "", "", 0, 0, ""),
                                        new Token(0, "", "", 0, 0, ""),
                                        new RangeNode(
                                            new Token(TokenType.LeftBracket, "", "", 0, 0, ""),
                                            new Token(TokenType.RightBracket, "", "", 0, 0, ""),
                                            new Token(TokenType.Semicolon, "", "", 0, 0, ""),
                                            new ValueNode(
                                                new Token(TokenType.Number, "2", "", 0, 0, "")
                                                ),
                                            new ValueNode(
                                                new Token(TokenType.Number, "4", "", 0, 0, "")
                                                ),
                                            true,
                                            true
                                        ),
                                        new ValueNode(
                                            new Token(TokenType.Number, "7", "", 0, 0, "")
                                        )
                                    ),
                                    new CellNode ( // (2, 4)
                                        new Token(0, "", "", 0, 0, ""),
                                        new Token(0, "", "", 0, 0, ""),
                                        new Token(0, "", "", 0, 0, ""),
                                        new ValueNode(
                                            new Token(TokenType.Number, "2", "", 0, 0, "")
                                        ),
                                        new ValueNode(
                                            new Token(TokenType.Number, "4", "", 0, 0, "")
                                        )
                                    )
                                }
                            ),
                             new FunctionCallNode(
                                new Token(TokenType.Identifier, "Union", "", 0, 0, ""),
                                new List<ArgumentNode>
                                {
                                    new CellNode ( // ([5 ; 8], 2)
                                        new Token(0, "", "", 0, 0, ""),
                                        new Token(0, "", "", 0, 0, ""),
                                        new Token(0, "", "", 0, 0, ""),
                                        new RangeNode(
                                            new Token(TokenType.LeftBracket, "", "", 0, 0, ""),
                                            new Token(TokenType.RightBracket, "", "", 0, 0, ""),
                                            new Token(TokenType.Semicolon, "", "", 0, 0, ""),
                                            new ValueNode(
                                                new Token(TokenType.Number, "5", "", 0, 0, "")
                                                ),
                                            new ValueNode(
                                                new Token(TokenType.Number, "8", "", 0, 0, "")
                                                ),
                                            false,
                                            false
                                        ),
                                        new ValueNode(
                                            new Token(TokenType.Number, "2", "", 0, 0, "")
                                        )
                                    ),
                                    new CellNode ( // (9, 2)
                                        new Token(0, "", "", 0, 0, ""),
                                        new Token(0, "", "", 0, 0, ""),
                                        new Token(0, "", "", 0, 0, ""),
                                        new ValueNode(
                                            new Token(TokenType.Number, "9", "", 0, 0, "")
                                        ),
                                        new ValueNode(
                                            new Token(TokenType.Number, "2", "", 0, 0, "")
                                        )
                                    )
                                }
                            )
                        },
                        new List<ParameterNode>()
                    )
                },
                new List<ParameterNode>()
            )
        );

        // Act
        var board = Generator.GetBoardFromAST(program);

        // Assert
        Console.WriteLine(board.ToString());
        Assert.That(board.Units.Count, Is.EqualTo(5));
        Assert.That(board[9, 5].Digit, Is.EqualTo(6));
        Assert.That(board[3, 6].Digit, Is.EqualTo(2));
        Assert.That(board[2, 7].Digit, Is.EqualTo(0));
        Assert.That(board[3, 7].Digit, Is.EqualTo(0));
        Assert.That(board[4, 7].Digit, Is.EqualTo(0));
        Assert.That(board[2, 4].Digit, Is.EqualTo(0));
        Assert.That(board[6, 2].Digit, Is.EqualTo(0));
        Assert.That(board[7, 2].Digit, Is.EqualTo(0));

    }
}