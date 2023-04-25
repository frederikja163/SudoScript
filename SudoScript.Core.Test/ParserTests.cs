using NUnit.Framework;
using SudoScript.Core;
using SudoScript.Core.Ast;

namespace SudoScript.Core.Test;

internal sealed class ParserTests
{
    // Unit
    [Test]
    public void ParseUnitTestWithoutName()
    {
        using StreamReader stream = new StreamReader("./TestData/UnitWithoutName.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        if (tree.Child.UnitStatements.First() is UnitNode unitNode)
        {
            Assert.That(unitNode.NameToken, Is.EqualTo(null));
            Assert.That(unitNode.Parameters.Count, Is.EqualTo(0));
        }
        else
        {
            Assert.Fail();
        }
    }

    [Test]
    public void ParseUnitTestWithName()
    {
        using StreamReader stream = new StreamReader("./TestData/UnitWithName.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        if (tree.Child.UnitStatements.First() is UnitNode unitNode)
        {
            Assert.That(unitNode.NameToken?.Match, Is.EqualTo("UnitName"));
            Assert.That(unitNode.Parameters.Count, Is.EqualTo(0));
        }
        else
        {
            Assert.Fail();
        }
    }

    [Test]
    public void ParseUnitTestWithParams()
    {
        using StreamReader stream = new StreamReader("./TestData/UnitWithParams.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        if (tree.Child.UnitStatements.First() is UnitNode unitNode)
        {
            Assert.That(unitNode.NameToken?.Match, Is.EqualTo("UnitName"));
            Assert.That(unitNode.Parameters.Count, Is.EqualTo(2));
        }
        else
        {
            Assert.Fail();
        }
    }

    // Rules
    [Test]
    public void ParseRuleTest()
    {
        using StreamReader stream = new StreamReader("./TestData/EmptyRuleUnit.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(0));
    }

    [Test]
    public void ParseRuleTestWithFunctionNoArgs()
    {
        using StreamReader stream = new StreamReader("./TestData/RuleUnitWithFuncNoArgs.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(1));
    }

    [Test]
    public void ParseRuleTestWithFunctionWithArgs()
    {
        using StreamReader stream = new StreamReader("./TestData/RuleUnitWithFuncWithArgs.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(1));
    }

    [Test]
    public void ParseRuleTestWithFunctionMultipleArgs()
    {
        using StreamReader stream = new StreamReader("./TestData/RuleUnitWithFuncMultipleArgs.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(2));
    }

    // Givens
    [Test]
    public void ParseGivensNoGivens()
    {
        using StreamReader stream = new StreamReader("./TestData/GivensNoGiven.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(0));
    }

    [Test]
    public void ParseGivensWithGiven()
    {
        using StreamReader stream = new StreamReader("./TestData/GivensWithGiven.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(1));
    }

    // FunctionCall
    [Test]
    public void ParseFunctionCallNoArgs()
    {
        using StreamReader stream = new StreamReader("./TestData/CallFunctionNoArgs.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(0));
    }

    [Test]
    public void ParseFunctionCallWithArgs()
    {
        using StreamReader stream = new StreamReader("./TestData/CallFunctionWithArgs.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(2));
    }

    [Test]
    public void ParseCellFunctionCall()
    {
        using StreamReader stream = new StreamReader("./TestData/CellFunctionCall.txt");
        ProgramNode tree = Parser.ParseProgram(stream);

        if (tree.Child.UnitStatements[0] is UnitNode unitNode)
        {
            if(unitNode.UnitStatements[0] is FunctionCallNode callNode) 
            {
                if (callNode.Arguments[0] is CellNode cellNode)
                {
                    Assert.Pass();
                }
                else
                {
                    Assert.Fail();
                }
            }
            else 
            {
                Assert.Fail();
            }
        }
        else
        {
            Assert.Fail();
        }
    }
}
