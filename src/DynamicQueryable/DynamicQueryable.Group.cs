using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;
using VarType = System.Collections.Generic.IDictionary<string, object?>;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic;

public static partial class DynamicQueryable {

    public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, string resultSelector, params object[] values)
        => GroupBy(source, keySelector, elementSelector, resultSelector, null, null, values);

    public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, string resultSelector, Settings settings, params object[] values)
        => GroupBy(source, keySelector, elementSelector, resultSelector, null, settings, values);

    public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, string resultSelector, VarType variables, params object[] values)
        => GroupBy(source, keySelector, elementSelector, resultSelector, variables, null, values);

    public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, string resultSelector, VarType? variables, Settings? settings, params object[] values) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (string.IsNullOrWhiteSpace(keySelector)) throw new ArgumentNullException(nameof(keySelector));
        if (string.IsNullOrWhiteSpace(elementSelector)) throw new ArgumentNullException(nameof(elementSelector));
        if (string.IsNullOrWhiteSpace(resultSelector)) throw new ArgumentNullException(nameof(resultSelector));

        var keyLambda = Evaluator.ToLambda(keySelector, [source.ElementType], variables, settings, values);
        var elementLambda = Evaluator.ToLambda(elementSelector, [source.ElementType], variables, settings, values);
        var enumElementType = typeof(IEnumerable<>).MakeGenericType(elementLambda.Body.Type);
        var resultLambda = Evaluator.ToLambda(resultSelector, [keyLambda.Body.Type, enumElementType], variables, settings, values);

        return source.Provider.CreateQuery(
            Expression.Call(
                typeof(Queryable),
                "GroupBy",
                [source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type, resultLambda.Body.Type],
                source.Expression,
                Expression.Quote(keyLambda),
                Expression.Quote(elementLambda),
                Expression.Quote(resultLambda)
            )
        );
    }

    public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector, params object[] values)
        => GroupBy(source, keySelector, resultSelector, (VarType?)null, null, values);

    public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector, Settings settings, params object[] values)
        => GroupBy(source, keySelector, resultSelector, (VarType?)null, settings, values);

    public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector, VarType variables, params object[] values)
        => GroupBy(source, keySelector, resultSelector, variables, null, values);

    public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector, VarType? variables, Settings? settings, params object[] values) {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (string.IsNullOrWhiteSpace(keySelector)) throw new ArgumentNullException(nameof(keySelector));
        if (string.IsNullOrWhiteSpace(resultSelector)) throw new ArgumentNullException(nameof(resultSelector));

        var keyLambda = Evaluator.ToLambda(keySelector, [source.ElementType], variables, settings, values);
        var enumSourceType = typeof(IEnumerable<>).MakeGenericType(source.ElementType);
        var resultLambda = Evaluator.ToLambda(resultSelector, [keyLambda.Body.Type, enumSourceType], variables, settings, values);

        return source.Provider.CreateQuery(
            Expression.Call(
                typeof(Queryable),
                "GroupBy",
                [source.ElementType, keyLambda.Body.Type, resultLambda.Body.Type],
                source.Expression,
                Expression.Quote(keyLambda),
                Expression.Quote(resultLambda)
            )
        );
    }

    public static IQueryable GroupBy(this IQueryable source, string keySelector, params object[] values)
        => GroupBy(source, keySelector, (VarType?)null, null, values);

    public static IQueryable GroupBy(this IQueryable source, string keySelector, Settings settings, params object[] values)
        => GroupBy(source, keySelector, (VarType?)null, settings, values);

    public static IQueryable GroupBy(this IQueryable source, string keySelector, VarType variables, params object[] values)
        => GroupBy(source, keySelector, variables, null, values);

    public static IQueryable GroupBy(this IQueryable source, string keySelector, VarType? variables, Settings? settings, params object[] values)
        => HandleLambda(source, "GroupBy", keySelector, true, variables, values, settings);
}