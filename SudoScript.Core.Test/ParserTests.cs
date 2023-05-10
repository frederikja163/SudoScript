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
        string testString = @"
unit {

}";

        ProgramNode tree = Parser.ParseProgram(testString);

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
        string testString = @"
unit UnitName {

}";

        ProgramNode tree = Parser.ParseProgram(testString);

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
        string testString = @"
unit UnitName a (x,y) {
	
}";

        ProgramNode tree = Parser.ParseProgram(testString);

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
        string testString = @"
rules {

}";

        ProgramNode tree = Parser.ParseProgram(testString);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(0));
    }

    [Test]
    public void ParseRuleTestWithFunctionNoArgs()
    {
        string testString = @"
rules {
	myRule 
}";

        ProgramNode tree = Parser.ParseProgram(testString);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(1));
    }

    [Test]
    public void ParseRuleTestWithFunctionWithArgs()
    {
        string testString = @"
rules {
	myRule 1 (3,4)
}";

        ProgramNode tree = Parser.ParseProgram(testString);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(1));
    }

    [Test]
    public void ParseRuleTestWithFunctionMultipleArgs()
    {
        string testString = @"
rules {
	myRule 1 (3,4)
	secondRule
}";

        ProgramNode tree = Parser.ParseProgram(testString);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(2));
    }

    // Givens
    [Test]
    public void ParseGivensNoGivens()
    {
        string testString = @"
givens {

}";
 
        ProgramNode tree = Parser.ParseProgram(testString);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(0));
    }

    [Test]
    public void ParseGivensWithGiven()
    {
        string testString = @"
givens {
	(2,3) 5
}";

        ProgramNode tree = Parser.ParseProgram(testString);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(1));
    }

    // FunctionCall
    [Test]
    public void ParseFunctionCallNoArgs()
    {
        string testString = @"
MyFunc";

        ProgramNode tree = Parser.ParseProgram(testString);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(0));
    }

    [Test]
    public void ParseFunctionCallWithArgs()
    {
        string testString = @"
MyFunc 10 (1,2)";

        ProgramNode tree = Parser.ParseProgram(testString);

        Assert.That(tree.Child.UnitStatements[0].Children().Count, Is.EqualTo(2));
    }

    [Test]
    public void ParseCellFunctionCall()
    {
        string testString = @"
unit {
(9,5)	
}";

        ProgramNode tree = Parser.ParseProgram(testString);

        if (tree.Child.UnitStatements[0] is UnitNode unitNode)
        {
            if(unitNode.UnitStatements[0] is FunctionCallNode callNode) 
            {
                Assert.IsInstanceOf<CellNode>(callNode.Arguments[0]);
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

    [Test]
    public void ProgramEndsWithNewline()
    {
        string testString = @"
unit UnitName a (b,c) {
    rules {
    
    }
}
";

        Assert.DoesNotThrow(() => Parser.ParseProgram(testString));
    }

    [Test]
    public void ProgramEndsWithSpace()
    {
        string testString = @"
unit UnitName a (b,c) {
    rules {
    
    }
} ";

        Assert.DoesNotThrow(() => Parser.ParseProgram(testString));
    }

    [Test]
    public void ProgramEndsWithLineComment()
    {
        string testString = @"
unit UnitName a (b,c) {
    rules {
    
    }
} //end";

        Assert.DoesNotThrow(() => Parser.ParseProgram(testString));
    }

    [Test]
    public void ProgramEndsWithBlockComment()
    {
        string testString = @"
unit UnitName a (b,c) {
    rules {
    
    }
}
/*
end
*/";

        Assert.DoesNotThrow(() => Parser.ParseProgram(testString));
    }

    [Test]
    public void EmptyFile()
    {
        string testString = "";

        Assert.DoesNotThrow(() => Parser.ParseProgram(testString));
    }
}
