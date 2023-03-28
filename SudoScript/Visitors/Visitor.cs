using SudoScript.Ast;

namespace SudoScript.Visitors;

public interface Visitor
{
    public void Visit(IAstNode node);
    public void Visit(ProgramNode node);
    public void Visit(UnitNode node);
    public void Visit(ArgumentNode node);
    public void Visit(ParameterCellNode node);
    public void Visit(ParameterIdentifierNode node);
    public void Visit(UnitStatementNode node);
    public void Visit(FunctionCallNode node);
    public void Visit(CellNode node);
    public void Visit(ExpressionNode node);
    public void Visit(RangeNode node);
    public void Visit(BinaryNode node);
    public void Visit(UnaryNode node);
    public void Visit(IdentifierNode node);
    public void Visit(RulesNode node);
    public void Visit(GivensNode node);
    public void Visit(GivensStatementNode node);
}

public abstract class VisitorBase : Visitor
{
    public virtual void Visit(IAstNode node)
    {
        switch (node)
        {
            case ProgramNode programNode:
                Visit(programNode);
                break;
            case UnitNode unitNode:
                Visit(unitNode);
                Visit((UnitStatementNode)unitNode);
                break;
            case ParameterCellNode parameterCellNode:
                Visit(parameterCellNode);
                Visit((ParameterNode)parameterCellNode);
                break;
            case ParameterIdentifierNode parameterIdentifierNode:
                Visit(parameterIdentifierNode);
                Visit((ParameterNode)parameterIdentifierNode);
                break;
            case FunctionCallNode functionCallNode:
                Visit(functionCallNode);
                Visit((UnitStatementNode)functionCallNode);
                break;
            case CellNode cellNode:
                Visit(cellNode);
                Visit((ArgumentNode)cellNode);
                break;
            case RangeNode rangeNode:
                Visit(rangeNode);
                Visit((ArgumentNode)rangeNode);
                Visit((ExpressionNode)rangeNode);
                break;
            case BinaryNode binaryNode:
                Visit(binaryNode);
                Visit((ArgumentNode)binaryNode);
                Visit((ExpressionNode)binaryNode);
                break;
            case UnaryNode unaryNode:
                Visit(unaryNode);
                Visit((ArgumentNode)unaryNode);
                Visit((ExpressionNode)unaryNode);
                break;
            case IdentifierNode identifierNode:
                Visit(identifierNode);
                Visit((ArgumentNode)identifierNode);
                Visit((ExpressionNode)identifierNode);
                break;
            case RulesNode rulesNode:
                Visit(rulesNode);
                Visit((UnitStatementNode)rulesNode);
                break;
            case GivensNode givensNode:
                Visit(givensNode);
                Visit((UnitStatementNode)givensNode);
                break;
            case GivensStatementNode givensStatementNode:
                Visit(givensStatementNode);
                break;
            default:
                break;
        }

        foreach (IAstNode children in node.Children())
        {
            Visit(children);
        }
    }

    public virtual void Visit(ProgramNode node)
    {
    }

    public virtual void Visit(UnitNode node)
    {
    }

    public virtual void Visit(ArgumentNode node)
    {
    }

    public virtual void Visit(ParameterCellNode node)
    {
    }

    public virtual void Visit(ParameterIdentifierNode node)
    {
    }

    public virtual void Visit(UnitStatementNode node)
    {
    }

    public virtual void Visit(FunctionCallNode node)
    {
    }

    public virtual void Visit(CellNode node)
    {
    }

    public virtual void Visit(ExpressionNode node)
    {
    }

    public virtual void Visit(RangeNode node)
    {
    }

    public virtual void Visit(BinaryNode node)
    {
    }

    public virtual void Visit(UnaryNode node)
    {
    }

    public virtual void Visit(IdentifierNode node)
    {
    }

    public virtual void Visit(RulesNode node)
    {
    }

    public virtual void Visit(GivensNode node)
    {
    }

    public virtual void Visit(GivensStatementNode node)
    {
    }
}
