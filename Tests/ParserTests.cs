using NUnit.Framework;
using SudoScript;
using SudoScript.Ast;

namespace Tests;

internal sealed class ParserTests
{
    [Test]
    public void ParseUnitTestWithoutName()
    {
        using StreamReader stream = new StreamReader("./TestData/UnitWithoutName.txt");
        var tree = Parser.ParseProgram(stream);

        UnitNode testNode = new UnitNode(null, new(), new());

        Assert.That(tree.Child.Equals(testNode));
    }

    [Test]
    public void ParseUnitTestWithName()
    {
        using StreamReader stream = new StreamReader("./TestData/UnitWithName.txt");
        var tree = Parser.ParseProgram(stream);

        Token nameToken = new Token(TokenType.Identifier, "UnitName", "unit UnitName {\n", 1, 6, "UnitWithName.txt");
        UnitNode testNode = new UnitNode(nameToken, new(), new());

        Assert.That(tree.Child.Equals(testNode));
    }

    [Test]
    public void ParseUnitTestWithParams()
    {
        using StreamReader stream = new StreamReader("./TestData/UnitWithParams.txt");
        var tree = Parser.ParseProgram(stream);

        Token nameToken = new Token(TokenType.Identifier, "UnitName", "unit UnitName {\n", 1, 6, "UnitWithParams.txt");
        List<ParameterNode> children = new List<ParameterNode>();

        children.Add(new ParameterIdentifierNode(new Token(TokenType.Identifier, "a", "unit UnitName a (x,y) {\n", 1, 15, "UnitWithParams.txt")));

        ParameterIdentifierNode x = new ParameterIdentifierNode(new Token(TokenType.Identifier, "x", "unit UnitName a (x,y) {\n", 1, 18, "UnitWithParams.txt"));
        ParameterIdentifierNode y = new ParameterIdentifierNode(new Token(TokenType.Identifier, "y", "unit UnitName a (x,y) {\n", 1, 20, "UnitWithParams.txt"));
        children.Add(new ParameterCellNode(x,y));

        UnitNode testNode = new UnitNode(nameToken, new(), children);

        Assert.That(tree.Child.Equals(testNode));
    }
}
