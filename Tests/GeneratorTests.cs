using NUnit.Framework;
using SudoScript.Ast;
using SudoScript;

namespace Tests;

internal sealed class GeneratorTests
{
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
                                new Token(TokenType.Identifier, "union", "", 0, 0, ""),
                                new List<ArgumentNode> 
                                {
                                    new CellNode ( // (5, 7)
                                        new Token(0, "", "", 0, 0, ""), 
                                        new Token(0, "", "", 0, 0, ""), 
                                        new Token(0, "", "", 0, 0, ""), 
                                        new BinaryNode(
                                            new Token(TokenType.Plus, "", "", 0, 0, ""), 
                                            BinaryType.Plus, 
                                            new ValueNode(
                                                new Token(TokenType.Number, "3", "", 0, 0, "")
                                            ),
                                            new ValueNode(
                                                new Token(TokenType.Number, "2", "", 0, 0, "")
                                            )
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
        Assert.That(board.Units.Count, Is.EqualTo(2));
        Assert.That(board[9, 5], Is.EqualTo(6));
        Assert.That(board[3, 6], Is.EqualTo(2));
        Assert.That(board[5, 7], Is.EqualTo(0));
        Assert.That(board[2, 4], Is.EqualTo(0));
        
    }
}