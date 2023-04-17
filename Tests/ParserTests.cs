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

        Assert.That(tree.Child?.NameToken, Is.EqualTo(null));
        Assert.That(tree.Child?.Parameters?.Count, Is.EqualTo(0));
    }

    [Test]
    public void ParseUnitTestWithName()
    {
        using StreamReader stream = new StreamReader("./TestData/UnitWithName.txt");
        var tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child?.NameToken.Match, Is.EqualTo("UnitName"));
        Assert.That(tree.Child?.Parameters?.Count, Is.EqualTo(0));
    }

    [Test]
    public void ParseUnitTestWithParams()
    {
        using StreamReader stream = new StreamReader("./TestData/UnitWithParams.txt");
        var tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child?.NameToken.Match, Is.EqualTo("UnitName"));
        Assert.That(tree.Child?.Parameters?.Count, Is.EqualTo(2));
    }

    [Test]
    public void ParseRuleTest() 
    {
        using StreamReader stream = new StreamReader("./TestData/EmptyRuleUnit.txt");
        var tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements.Count, Is.EqualTo(1));
    }

    [Test]
    public void ParseRuleTestWithFunctionNoArgs()
    {
        using StreamReader stream = new StreamReader("./TestData/RuleUnitWithFuncNoArgs.txt");
        var tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(1));
    }

    [Test]
    public void ParseRuleTestWithFunctionWithArgs()
    {
        using StreamReader stream = new StreamReader("./TestData/RuleUnitWithFuncWithArgs.txt");
        var tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(1));
    }

    [Test]
    public void ParseRuleTestWithFunctionMultipleArgs()
    {
        using StreamReader stream = new StreamReader("./TestData/RuleUnitWithFuncMultipleArgs.txt");
        var tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(2));
    }
}
