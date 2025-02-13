using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConsoleApp1.Model
{
    public class OperationType
    {
        public Dictionary<string, Func<Expression, Expression, Expression>> Arguments { get; private set; }
        public OperationType()
        {
            // Initialize the dictionary with comparison operations
            Arguments = new Dictionary<string, Func<Expression, Expression, Expression>>
            {
                { "==", (left, right) => Expression.Equal(left, right) },
                { "!=", (left, right) => Expression.NotEqual(left, right) },
                { "<", (left, right) => Expression.LessThan(left, right) },
                { ">", (left, right) => Expression.GreaterThan(left, right) },
                { "<=", (left, right) => Expression.LessThanOrEqual(left, right) },
                { ">=", (left, right) => Expression.GreaterThanOrEqual(left, right) }
            };
        }

        public Expression ParseQuery(string query)
        {
            // Find the operator in the query by matching with keys in Arguments
            string operatorSymbol = null;
            foreach (var key in Arguments.Keys)
            {
                if (query.Contains(key))
                {
                    operatorSymbol = key;
                    break;
                }
            }

            // If no operator is found, throw an exception
            if (operatorSymbol == null)
            {
                throw new ArgumentException("Query must contain a valid operator.");
            }

            // Split the query into left operand and right operand based on the operator
            string[] parts = query.Split(new[] { operatorSymbol }, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                throw new ArgumentException("Query must be in the format 'left operator right'.");
            }

            string leftOperand = parts[0].Trim();
            string rightOperand = parts[1].Trim();

            // Create expressions for left and right operands
           
            Expression left = Expression.Variable(typeof(int), leftOperand);  // Adjust type as needed
            Expression right = Expression.Constant(int.Parse(rightOperand));   // Adjust parsing as needed

            // Check if the operator is valid and return the corresponding expression
            if (Arguments.TryGetValue(operatorSymbol, out var operation))
            {
                return operation(left, right);
            }

            throw new NotSupportedException($"Operator '{operatorSymbol}' is not supported.");
        }

        public Expression ParseQuery1<T>(string query, Func<string, T> parseOperand)
        {
            // Find the operator in the query by matching with keys in Arguments
            string operatorSymbol = null;
            foreach (var key in Arguments.Keys)
            {
                if (query.Contains(key))
                {
                    operatorSymbol = key;
                    break;
                }
            }

            // If no operator is found, throw an exception
            if (operatorSymbol == null)
            {
                throw new ArgumentException("Query must contain a valid operator.");
            }

            // Split the query into left operand and right operand based on the operator
            string[] parts = query.Split(new[] { operatorSymbol }, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                throw new ArgumentException("Query must be in the format 'property operator value'.");
            }

            string leftOperand = parts[0].Trim(); // This will be the property name (e.g., "Country")
            string rightOperand = parts[1].Trim(); // This will be the value to compare

            // Get the property from the type T (e.g., Address.Country)
            var propertyInfo = typeof(T).GetProperty(leftOperand);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{leftOperand}' does not exist on type '{typeof(T).Name}'.");
            }

            // Create the left-hand side expression (e.g., "x => x.Country")
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);

            // Parse the right-hand side operand
            var rightValue = parseOperand(rightOperand);
            var right = Expression.Constant(rightValue, propertyInfo.PropertyType);

            // Check if the operator is valid and return the corresponding expression
            if (Arguments.TryGetValue(operatorSymbol, out var operation))
            {
                return operation(property, right);
            }

            throw new NotSupportedException($"Operator '{operatorSymbol}' is not supported.");
        }
    }
}
