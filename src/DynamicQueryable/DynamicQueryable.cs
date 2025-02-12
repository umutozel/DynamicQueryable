using System.Linq.Expressions;
using Jokenizer.Net;
using VarType = System.Collections.Generic.IDictionary<string, object?>;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic;

public static partial class DynamicQueryable {

    private static Expression CreateLambda(IQueryable source, string method, string? expression, bool generic, VarType? variables, object[] values, Settings? settings) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (string.IsNullOrWhiteSpace(expression)) throw new ArgumentNullException(nameof(expression));

        var types = new[] { source.ElementType };
        var lambda = Evaluator.ToLambda(expression!, types, variables, settings, values);

        return Expression.Call(
            typeof(Queryable),
            method,
            generic ? [source.ElementType, lambda.Body.Type] : types,
            source.Expression,
            Expression.Quote(lambda)
        );
    }

    private static Expression CreateExpression(IQueryable source, string method, params Expression[] expressions) {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return Expression.Call(
            typeof(Queryable),
            method,
            [source.ElementType],
            new[] { source.Expression }.Concat(expressions).ToArray()
        );
    }

    private static IQueryable Handle(IQueryable source, string method) {
        var expression = CreateExpression(source, method);
        return source.Provider.CreateQuery(expression);
    }

    private static IQueryable HandleConstant(IQueryable source, string method, object value) {
        var expression = CreateExpression(source, method, Expression.Constant(value));
        return source.Provider.CreateQuery(expression);
    }

    private static IQueryable HandleLambda(IQueryable source, string method, string? expression, bool generic, VarType? variables, object[] values, Settings? settings) {
        var lambda = CreateLambda(source, method, expression, generic, variables, values, settings);
        return source.Provider.CreateQuery(lambda);
    }

    private static object? Execute(IQueryable source, string method) {
        var expression = CreateExpression(source, method);
        return source.Provider.Execute(expression);
    }

    private static object? ExecuteLambda(IQueryable source, string method, string? expression, bool generic, VarType? variables, object[] values, Settings? settings) {
        var lambda = CreateLambda(source, method, expression, generic, variables, values, settings);
        return source.Provider.Execute(lambda);
    }

    private static object? ExecuteOptionalExpression(IQueryable source, string method, string? expression, bool generic, VarType? variables, object[] values, Settings? settings)
        => string.IsNullOrEmpty(expression)
            ? Execute(source, method)
            : ExecuteLambda(source, method, expression, generic, variables, values, settings);

    private static object? ExecuteConstant(IQueryable source, string method, object value) {
        var expression = CreateExpression(source, method, Expression.Constant(value));
        return source.Provider.Execute(expression);
    }
}