using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleApp1.Model
{
    public class GeneralOperationType
    {
        // Dictionary to map operators to corresponding Expression methods
        public Dictionary<string, Func<Expression, Expression, Expression>> Arguments { get; private set; }

        public GeneralOperationType()
        {
            // Initialize the dictionary with supported comparison operations
            Arguments = new Dictionary<string, Func<Expression, Expression, Expression>>
{
    { "==", Expression.Equal },
    { "!=", Expression.NotEqual },
    { "<", Expression.LessThan },
    { ">", Expression.GreaterThan },
    { "<=", Expression.LessThanOrEqual },
    { ">=", Expression.GreaterThanOrEqual },
    { "StartsWith", (left, right) => Expression.Call(left, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), right) },
    { "EndsWith", (left, right) => Expression.Call(left, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), right) },
    { "Contains", (left, right) => Expression.Call(left, typeof(string).GetMethod("Contains", new[] { typeof(string) }), right) }
};

        }

        public Expression ParseQuery<T>(string query, ParameterExpression parameter)
        {
            string operatorSymbol = null;
            foreach (var key in Arguments.Keys)
            {
                if (query.Contains(key))
                {
                    operatorSymbol = key;
                    break;
                }
            }

            if (operatorSymbol == null)
            {
                throw new ArgumentException("Query must contain a valid operator.");
            }

            string[] parts = query.Split(new[] { operatorSymbol }, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                throw new ArgumentException("Query must be in the format 'Property Operator Value'.");
            }

            string leftOperand = parts[0].Trim();
            string rightOperand = parts[1].Trim();

            // Handle nested properties like "Role.Name"
            PropertyInfo propertyInfo = null;
            Type currentType = typeof(T);
            Expression propertyExpression = parameter; // Start with the parameter expression

            foreach (var propertyName in leftOperand.Split('.'))
            {
                propertyInfo = currentType.GetProperty(propertyName);
                if (propertyInfo == null)
                {
                    throw new ArgumentException($"Property '{leftOperand}' does not exist on type '{typeof(T).Name}'.");
                }

                // Build the nested property expression
                propertyExpression = Expression.Property(propertyExpression, propertyInfo);
                currentType = propertyInfo.PropertyType; // Move to the next nested type
            }

            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{leftOperand}' does not exist on type '{typeof(T).Name}'.");
            }

            // Ensure the right operand is parsed correctly for its type
            object parsedValue = Convert.ChangeType(rightOperand, propertyInfo.PropertyType);
            var right = Expression.Constant(parsedValue, propertyInfo.PropertyType);

            // Continue with your existing logic
            if (Arguments.TryGetValue(operatorSymbol, out var operation))
            {
                return operation(propertyExpression, right);
            }

            throw new NotSupportedException($"Operator '{operatorSymbol}' is not supported.");
        }


    }
}
