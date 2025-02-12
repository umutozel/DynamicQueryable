using System.Linq.Expressions;
using Jokenizer.Net;
using VarType = System.Collections.Generic.IDictionary<string, object?>;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic;

public static partial class DynamicQueryable {

    public static object Average(this IQueryable source, string? selector = null, params object[] values)
        => Average(source, selector, null, null, values);

    public static object Average(this IQueryable source, string selector, Settings settings, params object[] values)
        => Average(source, selector, null, settings, values);

    public static object Average(this IQueryable source, string selector, VarType variables, params object[] values)
        => Average(source, selector, variables, null, values);

    public static object Average(this IQueryable source, string? selector, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "Average", selector, false, variables, values, settings)!;

    public static object Sum(this IQueryable source, string? selector = null, params object[] values)
        => Sum(source, selector, null, null, values);

    public static object Sum(this IQueryable source, string selector, Settings settings, params object[] values)
        => Sum(source, selector, null, settings, values);

    public static object Sum(this IQueryable source, string selector, VarType variables, params object[] values)
        => Sum(source, selector, variables, null, values);

    public static object Sum(this IQueryable source, string? selector, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "Sum", selector, false, variables, values, settings)!;

    public static object Max(this IQueryable source, string? selector = null, params object[] values)
        => Max(source, selector, null, null, values);

    public static object Max(this IQueryable source, string selector, Settings settings, params object[] values)
        => Max(source, selector, null, settings, values);

    public static object Max(this IQueryable source, string selector, VarType variables, params object[] values)
        => Max(source, selector, variables, null, values);

    public static object Max(this IQueryable source, string? selector, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "Max", selector, true, variables, values, settings)!;

    public static object Min(this IQueryable source, string? selector = null, params object[] values)
        => Min(source, selector, null, null, values);

    public static object Min(this IQueryable source, string selector, Settings settings, params object[] values)
        => Min(source, selector, null, settings, values);

    public static object Min(this IQueryable source, string selector, VarType variables, params object[] values)
        => Min(source, selector, variables, null, values);

    public static object Min(this IQueryable source, string? selector, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "Min", selector, true, variables, values, settings)!;

    public static object Aggregate(this IQueryable source, string func, params object[] values)
        => Aggregate(source, func, null, null, values);

    public static object Aggregate(this IQueryable source, string func, Settings settings, params object[] values)
        => Aggregate(source, func, null, settings, values);

    public static object Aggregate(this IQueryable source, string func, VarType variables, params object[] values)
        => Aggregate(source, func, variables, null, values);

    public static object Aggregate(this IQueryable source, string func, VarType? variables, Settings? settings, params object[] values) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (string.IsNullOrWhiteSpace(func)) throw new ArgumentNullException(nameof(func));

        var funcLambda = Evaluator.ToLambda(func, [source.ElementType, source.ElementType], variables, settings, values);

        return source.Provider.Execute(
            Expression.Call(
                typeof(Queryable),
                "Aggregate",
                [source.ElementType],
                source.Expression,
                Expression.Quote(funcLambda)
            )
        );
    }

    public static object Aggregate(this IQueryable source, object seed, string func, params object[] values)
        => Aggregate(source, seed, func, (VarType?)null, null, values);

    public static object Aggregate(this IQueryable source, object seed, string func, Settings settings, params object[] values)
        => Aggregate(source, seed, func, (VarType?)null, settings, values);

    public static object Aggregate(this IQueryable source, object seed, string func, VarType variables, params object[] values)
        => Aggregate(source, seed, func, variables, null, values);

    public static object Aggregate(this IQueryable source, object seed, string func, VarType? variables, Settings? settings, params object[] values) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (seed == null) throw new ArgumentNullException(nameof(seed));
        if (string.IsNullOrWhiteSpace(func)) throw new ArgumentNullException(nameof(func));

        var funcLambda = Evaluator.ToLambda(func, [seed.GetType(), source.ElementType], variables, settings, values);

        return source.Provider.Execute(
            Expression.Call(
                typeof(Queryable),
                "Aggregate",
                [source.ElementType, seed.GetType()],
                source.Expression,
                Expression.Constant(seed),
                Expression.Quote(funcLambda)
            )
        );
    }

    public static object Aggregate(this IQueryable source, object seed, string func, string selector, params object[] values)
        => Aggregate(source, seed, func, selector, null, null, values);

    public static object Aggregate(this IQueryable source, object seed, string func, string selector, Settings settings, params object[] values)
        => Aggregate(source, seed, func, selector, null, settings, values);

    public static object Aggregate(this IQueryable source, object seed, string func, string selector, VarType variables, params object[] values)
        => Aggregate(source, seed, func, selector, variables, null, values);

    public static object Aggregate(this IQueryable source, object seed, string func, string selector, VarType? variables, Settings? settings, params object[] values) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (seed == null) throw new ArgumentNullException(nameof(seed));
        if (string.IsNullOrWhiteSpace(func)) throw new ArgumentNullException(nameof(func));
        if (string.IsNullOrWhiteSpace(selector)) throw new ArgumentNullException(nameof(func));

        var funcLambda = Evaluator.ToLambda(func, [seed.GetType(), source.ElementType], variables, settings, values);
        var selectorLambda = Evaluator.ToLambda(selector, [seed.GetType()], variables, settings, values);

        return source.Provider.Execute(
            Expression.Call(
                typeof(Queryable),
                "Aggregate",
                [source.ElementType, seed.GetType(), selectorLambda.Body.Type],
                source.Expression,
                Expression.Constant(seed),
                Expression.Quote(funcLambda),
                Expression.Quote(selectorLambda)
            )
        );
    }
}