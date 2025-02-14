using ConsoleApp1.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;


public class Role
{
    public string Name { get; set; }
    public int Age { get; set; }
    public float Age1 { get; set; }
}
public class Asset
{
    public string Location { get; set; }
    public int Age { get; set; }
    public float Age1 { get; set; }

    public Role Role { get; set; }
}


public class Program
{
    public static void Main()
    {

        var roles = new List<Role>
    {
        new Role { Name = "Alice", Age = 25, Age1 = 25.5f },
        new Role { Name = "Bob", Age = 30, Age1 = 30.7f },
        new Role { Name = "Charlie", Age = 35, Age1 = 35.2f },
        new Role { Name = "Diana", Age = 40, Age1 = 40.1f },
        new Role { Name = "Eve", Age = 45, Age1 = 45.8f }
    };

        List<Asset> assets = new List<Asset>
    {
        new Asset { Location = "New York", Age = 5, Age1 = 5.5f, Role=new Role { Name = "Alice1", Age = 25, Age1 = 25.5f } },
        new Asset { Location = "Los Angeles", Age = 10, Age1 = 10.2f,  Role=new Role { Name = "Alice1", Age = 25, Age1 = 25.5f }},
        new Asset { Location = "Chicago", Age = 3, Age1 = 3.7f, Role=new Role { Name = "Alice3", Age = 25, Age1 = 25.5f } }
    };

        var logicalType = new LogicalOperationType();
        var operationType = new OperationType();
        var generalOperation = new GeneralOperationType();

        var query = "Role.Name==Alice1";
        var tokens = Tokenize(query);

        var parameter = Expression.Parameter(typeof(Asset), "x"); // Single parameter for all expressions
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
                Expression res = generalOperation.ParseQuery<Asset>(token, parameter);
                expressions.Add(res);
            }
        }

        var generalExpression = logicalType.GenerateLogicalTreeExpression<Asset>(expressions, parameter);
        var xx = generalExpression.Compile();
        var result = assets.Where(xx).ToList();

        Console.WriteLine(result.Count); // Output the result count
        
    
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
}
