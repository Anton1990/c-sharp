using ConsoleApp1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;


public class Role
{
    public string Name { get; set; }
    public int  Age { get; set; }
}

public class Program
{
    public static void Main()
    {

        var logicalType = new LogicalOperationType();
        var operationType = new OperationType();
        string query = "myRole1e==1 OR myRole2e>2 OR myRole2e==2 AND myRole2e1!=2 OR myRole2e>2";
        query = "Age==1  OR Age==2 AND Age==1";
        var tokens = Tokenize(query);


        List<Expression> expressions = new List<Expression>();
        foreach (var token in tokens)
        {
                if (logicalType.LogicalArguments.ContainsKey(token))
                {
                var res = logicalType.GenerateLogicalExpression(token);
                expressions.Add(res);
                }
                else
                {
                    // Parse the token as a comparison or other operation
                    Expression res = operationType.ParseQuery(token);
                    expressions.Add(res);
                }
        }
        var generalExpression = logicalType.GenerateLogicalTreeExpression(expressions);
   Console.ReadLine();

    }

    public static Expression<Func<Role, bool>> ParseQuery(string query)
    {
        var parameter = Expression.Parameter(typeof(Role), "role");
        var tokens = Tokenize(query);
        var expression = BuildExpression(tokens, parameter);
        return Expression.Lambda<Func<Role, bool>>(expression, parameter);
    }

    private static List<string> Tokenize(string query)
    {
        var tokens = new List<string>();
        var currentToken = string.Empty;

        foreach (char c in query)
        {
            if (char.IsWhiteSpace(c))
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken);
                    currentToken = string.Empty;
                }
            }
            else if (c == '(' || c == ')' || c == '|' || c == '&')
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken);
                    currentToken = string.Empty;
                }
                tokens.Add(c.ToString());
            }
            else
            {
                currentToken += c;
            }
        }

        if (currentToken.Length > 0)
        {
            tokens.Add(currentToken);
        }

        return tokens;
    }

    private static Expression BuildExpression(List<string> tokens, ParameterExpression parameter)
    {
        Stack<Expression> expressionStack = new Stack<Expression>();
        Stack<string> operatorStack = new Stack<string>();

        for (int i = 0; i < tokens.Count; i++)
        {
            string token = tokens[i];

            if (token == "OR" || token == "AND")
            {
                while (operatorStack.Count > 0 && GetPrecedence(operatorStack.Peek()) >= GetPrecedence(token))
                {
                    expressionStack.Push(ApplyOperator(operatorStack.Pop(), expressionStack));
                }
                operatorStack.Push(token);
            }
            else if (token == "(")
            {
                operatorStack.Push(token);
            }
            else if (token == ")")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    expressionStack.Push(ApplyOperator(operatorStack.Pop(), expressionStack));
                }

                if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                {
                    operatorStack.Pop(); // Pop the '('
                }
                else
                {
                    throw new InvalidOperationException("Mismatched parentheses in query.");
                }
            }
            else
            {
                // Handle role name or method call
                if (token.StartsWith("Name.Contains"))
                {
                    // Ensure the token is not null or empty
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        throw new InvalidOperationException("Token is null or empty.");
                    }

                    var methodParts = token.Split(new[] { '(' }, 2);
                    if (methodParts.Length == 2||true)
                    {
                        var methodArg = methodParts[0].Trim(')', '\'');
                        var property = Expression.Property(parameter, nameof(Role.Name));
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        var methodCall = Expression.Call(property, containsMethod, Expression.Constant(methodArg));
                        expressionStack.Push(methodCall);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid method format: {token}. Expected format: Name.Contains('argument').");
                    }
                }
                else
                {
                    var roleName = Expression.Constant(token);
                    var equality = Expression.Equal(Expression.Property(parameter, nameof(Role.Name)), roleName);
                    expressionStack.Push(equality);
                }
            }
        }

        while (operatorStack.Count > 0)
        {
            expressionStack.Push(ApplyOperator(operatorStack.Pop(), expressionStack));
        }

        if (expressionStack.Count != 1)
        {
            throw new InvalidOperationException("Invalid expression; check your query syntax.");
        }

        return expressionStack.Pop();
    }

    private static int GetPrecedence(string op)
    {
        switch (op)
        {
            case "AND": return 1;
            case "OR": return 0;
            default: return -1;
        }
    }

    private static Expression ApplyOperator(string op, Stack<Expression> expressionStack)
    {
        if (expressionStack.Count < 2)
        {
            throw new InvalidOperationException("Not enough operands for the operator.");
        }

        var right = expressionStack.Pop();
        var left = expressionStack.Pop();
        return op == "OR" ? Expression.OrElse(left, right) : Expression.AndAlso(left, right);
    }
}
