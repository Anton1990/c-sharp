using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;

public class LogicalOperationType
{
    // Separate dictionary for logical operations
    public Dictionary<string, Func<Expression, Expression, Expression>> LogicalArguments { get; private set; }

    // Parameter expressions for left and right operands
    public ParameterExpression Left { get; private set; }
    public ParameterExpression Right { get; private set; }

    public LogicalOperationType()
    {
        // Initialize the dictionary with logical operations
        LogicalArguments = new Dictionary<string, Func<Expression, Expression, Expression>>
        {
            { "AND", Expression.And }, // Logical AND
            { "OR",  Expression.Or }   // Logical OR
        };

        // Initialize default parameter expressions
        Left = Expression.Parameter(typeof(bool), "x");
        Right = Expression.Parameter(typeof(bool), "y");
    }

    // Method to generate a logical expression for a given operator
    public Expression GenerateLogicalExpression(string operation)
    {
        if (LogicalArguments.ContainsKey(operation))
        {
            // Use the delegate to create a logical expression
            return LogicalArguments[operation](Left, Right);
        }
        else
        {
            throw new InvalidOperationException($"Logical operation '{operation}' is not defined.");
        }
    }

    public Expression<Func<T, bool>> GenerateLogicalTreeExpression<T>(IEnumerable<Expression> expressions, ParameterExpression parameter)
    {
        var list = expressions.ToList();

        while (list.Any(x => x.NodeType == ExpressionType.And))
        {
            List<int> indices = list
                .Select((x, index) => new { Expression = x, Index = index })
                .Where(x => x.Expression.NodeType == ExpressionType.And)
                .Select(x => x.Index)
                .ToList();

            var left = list.ElementAt(indices.First() - 1);
            var right = list.ElementAt(indices.First() + 1);
            var expression = Expression.AndAlso(left, right);
            list[indices.First()] = expression;
            list.RemoveAt(indices.First() - 1);
            list.RemoveAt(indices.First());
        }

        Stack<Expression> stack = new Stack<Expression>();
        foreach (Expression expression in list.AsEnumerable().Reverse())
        {
            stack.Push(expression);
        }

        while (stack.Count > 2)
        {
            var left = stack.Pop();
            var expression = stack.Pop();
            var right = stack.Pop();

            if (expression.NodeType == ExpressionType.Or)
            {
                expression = Expression.OrElse(left, right);
            }
            else if (expression.NodeType == ExpressionType.And)
            {
                expression = Expression.AndAlso(left, right);
            }

            stack.Push(expression);
        }

        var finalExpression = stack.Pop();
        return Expression.Lambda<Func<T, bool>>(finalExpression, parameter);
    }


}
