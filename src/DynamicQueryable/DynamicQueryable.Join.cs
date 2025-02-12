using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;
using VarType = System.Collections.Generic.IDictionary<string, object?>;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic;

public static partial class DynamicQueryable {

    public static IQueryable Join(this IQueryable outer, IQueryable inner, string outerKeySelector, string innerKeySelector, string resultSelector, params object[] values)
        => Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null, null, values);

    public static IQueryable Join(this IQueryable outer, IQueryable inner, string outerKeySelector, string innerKeySelector, string resultSelector, Settings settings, params object[] values)
        => Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null, settings, values);

    public static IQueryable Join(this IQueryable outer, IQueryable inner, string outerKeySelector, string innerKeySelector, string resultSelector, VarType variables, params object[] values)
        => Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, variables, null, values);

    public static IQueryable Join(this IQueryable outer, IQueryable inner, string outerKeySelector, string innerKeySelector, string resultSelector, VarType? variables, Settings? settings, params object[] values) {
        if (outer == null) throw new ArgumentNullException(nameof(outer));
        if (inner == null) throw new ArgumentNullException(nameof(inner));
        if (string.IsNullOrWhiteSpace(outerKeySelector)) throw new ArgumentNullException(nameof(outerKeySelector));
        if (string.IsNullOrWhiteSpace(innerKeySelector)) throw new ArgumentNullException(nameof(innerKeySelector));
        if (string.IsNullOrWhiteSpace(resultSelector)) throw new ArgumentNullException(nameof(resultSelector));

        var outerKeyLambda = Evaluator.ToLambda(outerKeySelector, [outer.ElementType], variables, settings, values);
        var innerKeyLambda = Evaluator.ToLambda(innerKeySelector, [inner.ElementType], variables, settings, values);
        var resultLambda = Evaluator.ToLambda(resultSelector, [outer.ElementType, inner.ElementType], variables, settings, values);

        return outer.Provider.CreateQuery(
            Expression.Call(
                typeof(Queryable),
                "Join",
                [outer.ElementType, inner.ElementType, outerKeyLambda.Body.Type, resultLambda.Body.Type],
                outer.Expression,
                Expression.Constant(inner),
                Expression.Quote(outerKeyLambda),
                Expression.Quote(innerKeyLambda),
                Expression.Quote(resultLambda)
            )
        );
    }

    public static IQueryable GroupJoin(this IQueryable outer, IQueryable inner, string outerKeySelector, string innerKeySelector, string resultSelector, params object[] values)
        => GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null, null, values);

    public static IQueryable GroupJoin(this IQueryable outer, IQueryable inner, string outerKeySelector, string innerKeySelector, string resultSelector, Settings settings, params object[] values)
        => GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null, settings, values);

    public static IQueryable GroupJoin(this IQueryable outer, IQueryable inner, string outerKeySelector, string innerKeySelector, string resultSelector, VarType variables, params object[] values)
        => GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, variables, null, values);

    public static IQueryable GroupJoin(this IQueryable outer, IQueryable inner, string outerKeySelector, string innerKeySelector, string resultSelector, VarType? variables, Settings? settings, params object[] values) {
        if (outer == null) throw new ArgumentNullException(nameof(outer));
        if (inner == null) throw new ArgumentNullException(nameof(inner));
        if (string.IsNullOrWhiteSpace(outerKeySelector)) throw new ArgumentNullException(nameof(outerKeySelector));
        if (string.IsNullOrWhiteSpace(innerKeySelector)) throw new ArgumentNullException(nameof(innerKeySelector));
        if (string.IsNullOrWhiteSpace(resultSelector)) throw new ArgumentNullException(nameof(resultSelector));

        var outerKeyLambda = Evaluator.ToLambda(outerKeySelector, [outer.ElementType], variables, settings, values);
        var innerKeyLambda = Evaluator.ToLambda(innerKeySelector, [inner.ElementType], variables, settings, values);
        var innerEnumType = typeof(IEnumerable<>).MakeGenericType(inner.ElementType);
        var resultLambda = Evaluator.ToLambda(resultSelector, [outer.ElementType, innerEnumType], variables, settings, values);

        return outer.Provider.CreateQuery(
            Expression.Call(
                typeof(Queryable),
                "GroupJoin",
                [outer.ElementType, inner.ElementType, outerKeyLambda.Body.Type, resultLambda.Body.Type],
                outer.Expression,
                Expression.Constant(inner),
                Expression.Quote(outerKeyLambda),
                Expression.Quote(innerKeyLambda),
                Expression.Quote(resultLambda)
            )
        );
    }

    public static IQueryable Zip<T>(this IQueryable first, IEnumerable<T> second, string resultSelector, params object[] values)
        => Zip(first, second, resultSelector, null, null, values);

    public static IQueryable Zip<T>(this IQueryable first, IEnumerable<T> second, string resultSelector, Settings settings, params object[] values)
        => Zip(first, second, resultSelector, null, settings, values);

    public static IQueryable Zip<T>(this IQueryable first, IEnumerable<T> second, string resultSelector, VarType variables, params object[] values)
        => Zip(first, second, resultSelector, variables, null, values);

    public static IQueryable Zip<T>(this IQueryable first, IEnumerable<T> second, string resultSelector, VarType? variables, Settings? settings, params object[] values) {
        if (first == null) throw new ArgumentNullException(nameof(first));
        if (second == null) throw new ArgumentNullException(nameof(second));
        if (string.IsNullOrWhiteSpace(resultSelector)) throw new ArgumentNullException(nameof(resultSelector));

        var secondType = typeof(T);
        var resultLambda = Evaluator.ToLambda(resultSelector, [first.ElementType, secondType], variables, settings, values);

        return first.Provider.CreateQuery(
            Expression.Call(
                typeof(Queryable),
                "Zip",
                [first.ElementType, secondType, resultLambda.Body.Type],
                first.Expression,
                Expression.Constant(second),
                Expression.Quote(resultLambda)
            )
        );
    }
}