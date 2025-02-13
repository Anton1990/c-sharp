using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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

            var propertyInfo = typeof(T).GetProperty(leftOperand);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{leftOperand}' does not exist on type '{typeof(T).Name}'.");
            }

            var property = Expression.Property(parameter, propertyInfo);

            // Ensure the right operand is parsed correctly for string-based operations
            object parsedValue = propertyInfo.PropertyType == typeof(string) ? rightOperand : Convert.ChangeType(rightOperand, propertyInfo.PropertyType);
            var right = Expression.Constant(parsedValue, propertyInfo.PropertyType);

            if (Arguments.TryGetValue(operatorSymbol, out var operation))
            {
                return operation(property, right);
            }

            throw new NotSupportedException($"Operator '{operatorSymbol}' is not supported.");
        }


    }
}
