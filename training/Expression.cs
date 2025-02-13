using ConsoleApp1.Model;
using System.Linq.Expressions;
namespace DynamicQueriesInCSharp.Expressions
{
    public static class PredicateExpression<T>
    {
        // Equal (==)
        public static Expression<Func<T, bool>> CreateEqualExpression(string propertyName, object value)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var member = Expression.Property(param, propertyName);
            var constant = Expression.Constant(value);
            var body = Expression.Equal(member, constant);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        // Not Equal (!=)
        public static Expression<Func<T, bool>> CreateNotEqualExpression(string propertyName, object value)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var member = Expression.Property(param, propertyName);
            var constant = Expression.Constant(value);
            var body = Expression.NotEqual(member, constant);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        // Greater Than (>)
        public static Expression<Func<T, bool>> CreateGreaterThanExpression(string propertyName, object value)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var member = Expression.Property(param, propertyName);
            var constant = Expression.Constant(value);
            var body = Expression.GreaterThan(member, constant);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        // Greater Than or Equal (>=)
        public static Expression<Func<T, bool>> CreateGreaterThanOrEqualExpression(string propertyName, object value)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var member = Expression.Property(param, propertyName);
            var constant = Expression.Constant(value);
            var body = Expression.GreaterThanOrEqual(member, constant);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        // Less Than (<)
        public static Expression<Func<T, bool>> CreateLessThanExpression(string propertyName, object value)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var member = Expression.Property(param, propertyName);
            var constant = Expression.Constant(value);
            var body = Expression.LessThan(member, constant);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        // Less Than or Equal (<=)
        public static Expression<Func<T, bool>> CreateLessThanOrEqualExpression(string propertyName, object value)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var member = Expression.Property(param, propertyName);
            var constant = Expression.Constant(value);
            var body = Expression.LessThanOrEqual(member, constant);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}