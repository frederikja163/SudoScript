
using SudoScript.Core.Ast;

namespace SudoScript.Core;

public static class ExpressionParser
{
    public static ExpressionNode Parse(TokenStream stream, bool singleElement = false)
    {
        Stack<(Token oper, int index)> operatorStack = new();
        Stack<(ExpressionNode expression, int index)> expressionStack = new();

        int expectingParenthesis = 0;
        bool expectingEndRange = false;

        bool reduce;
        bool firstPeek = true;

        int index = 0;
        while(!singleElement || firstPeek)
        {
            firstPeek = false;
            //Read token as expression and figure out if the expression has ended.
            if(!stream.Peek(false, out Token? pending) ||
                pending.Type == TokenType.RightParenthesis && expectingParenthesis == 0 ||
                pending.Type == TokenType.Newline ||
                pending.Type == TokenType.Comma)
            {
                break;
            }

            stream.Continue(false);

            if(pending.Type == TokenType.BlockComment || 
                pending.Type == TokenType.Space ||
                pending.Type == TokenType.LineComment)
            {
                continue;
            }

            index++;

            Evaluate(pending);
        }

        if(expressionStack.Count == 1)
        {
            return expressionStack.Peek().expression;
        }
        else
        {
            throw new ArgumentException("Expression could not be parsed!");
        }

        void Evaluate(Token pending)
        {
            //check if pending is an expression by itself

            ExpressionNode? pendingExpression = null;
            if(pending.Type == TokenType.Identifier)
            {
                pendingExpression = new IdentifierNode(pending);
                reduce = false;
            }
            else if(pending.Type == TokenType.Number)
            {
                pendingExpression = new ValueNode(pending);
                reduce = false;
            }
            else if(operatorStack.Count <= 0)
            {
                reduce = false;
            }
            else
            {

                //if both are operators
                //get stack top precedence
                //get token precedence

                //if equal get associativity/default for that precedence
                reduce = false;
            }

            if(reduce) //reduce 
            {

                Evaluate(pending);
            }
            else if(pendingExpression is not null)
            {
                expressionStack.Push((pendingExpression, index));
            }
            else
            {
                operatorStack.Push((pending, index));
            }
        }
    }

}