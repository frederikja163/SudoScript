using NUnit.Framework;
using SudoScript;
using SudoScript.Data;

namespace Tests;

public class TestRule1 : IRule
{
    public bool EliminateCandidates(Unit unit)
    {
        throw new NotImplementedException();
    }

    public bool ValidateRules(Unit unit)
    {
        throw new NotImplementedException();
    }
}
public class TestRule2 : IRule
{
    public CellReference Cell { get; }

    public int Digit { get; }

    public TestRule2(CellReference cell, int digit)
    {
        Cell = cell;
        Digit = digit;
    }
    
    public TestRule2(int digit, CellReference cell)
    {
        Cell = cell;
        Digit = digit;
    }

    public bool EliminateCandidates(Unit unit)
    {
        throw new NotImplementedException();
    }

    public bool ValidateRules(Unit unit)
    {
        throw new NotImplementedException();
    }
}

public class TestUnit1 : Unit
{
    public TestUnit1()
    {
    }
}
public class TestUnit2 : Unit
{
    public CellReference Cell { get; }

    public int Digit { get; }

    public TestUnit2(CellReference cell, int digit)
    {
        Cell = cell;
        Digit = digit;
    }
    
    public TestUnit2(int digit, CellReference cell)
    {
        Cell = cell;
        Digit = digit;
    }
}

internal sealed class PluginTests
{
    [Test]
    public void CreateEmptyRuleTest()
    {
        IRule rule = Plugins.CreateRule("TestRule1");
        Assert.IsInstanceOf<TestRule1>(rule);
    }

    [Test]
    public void CreateRuleWithCellAndDigitTest()
    {
        IRule rule = Plugins.CreateRule("TestRule2", new CellReference(1, 2), 3);
        Assert.IsInstanceOf<TestRule2>(rule);
        TestRule2 rule2 = (TestRule2)rule;
        Assert.That(rule2.Cell.X, Is.EqualTo(1));
        Assert.That(rule2.Cell.Y, Is.EqualTo(2));
        Assert.That(rule2.Digit, Is.EqualTo(3));
    }
    
    [Test]
    public void CreateRuleWithDigitAndCellTest()
    {
        IRule rule = Plugins.CreateRule("TestRule2", 3, new CellReference(1, 2));
        Assert.IsInstanceOf<TestRule2>(rule);
        TestRule2 rule2 = (TestRule2)rule;
        Assert.That(rule2.Cell.X, Is.EqualTo(1));
        Assert.That(rule2.Cell.Y, Is.EqualTo(2));
        Assert.That(rule2.Digit, Is.EqualTo(3));
    }

    [Test]
    public void InvalidOperatorsCreateRule()
    {
        Assert.Throws<Exception>(() => Plugins.CreateRule("TestRule2", 1, 2));
    }

    [Test]
    public void CreateEmptyUnitTest()
    {
        Unit rule = Plugins.CreateUnit("TestUnit1");
        Assert.IsInstanceOf<TestUnit1>(rule);
    }

    [Test]
    public void CreateUnitWithCellAndDigitTest()
    {
        Unit unit = Plugins.CreateUnit("TestUnit2", new CellReference(1, 2), 3);
        Assert.IsInstanceOf<TestUnit2>(unit);
        TestUnit2 unit2 = (TestUnit2)unit;
        Assert.That(unit2.Cell.X, Is.EqualTo(1));
        Assert.That(unit2.Cell.Y, Is.EqualTo(2));
        Assert.That(unit2.Digit, Is.EqualTo(3));
    }
    
    [Test]
    public void CreateUnitWithDigitAndCellTest()
    {
        Unit unit = Plugins.CreateUnit("TestUnit2", 3, new CellReference(1, 2));
        Assert.IsInstanceOf<TestUnit2>(unit);
        TestUnit2 unit2 = (TestUnit2)unit;
        Assert.That(unit2.Cell.X, Is.EqualTo(1));
        Assert.That(unit2.Cell.Y, Is.EqualTo(2));
        Assert.That(unit2.Digit, Is.EqualTo(3));
    }

    [Test]
    public void InvalidOperatorsCreateUnit()
    {
        Assert.Throws<Exception>(() => Plugins.CreateRule("TestUnit2", 1, 2));
    }
}
