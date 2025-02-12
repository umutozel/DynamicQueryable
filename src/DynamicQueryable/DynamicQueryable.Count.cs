using Jokenizer.Net;
using VarType = System.Collections.Generic.IDictionary<string, object?>;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic;

public static partial class DynamicQueryable {

    public static int Count(this IQueryable source, string? predicate = null, params object[] values)
        => Count(source, predicate, null, null, values);

    public static int Count(this IQueryable source, string predicate, Settings settings, params object[] values)
        => Count(source, predicate, null, settings, values);

    public static int Count(this IQueryable source, string predicate, VarType variables, params object[] values)
        => Count(source, predicate, variables, null, values);

    public static int Count(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => (int)ExecuteOptionalExpression(source, "Count", predicate, string.IsNullOrEmpty(predicate), variables, values, settings)!;

    public static long LongCount(this IQueryable source, string? predicate = null, params object[] values)
        => LongCount(source, predicate, null, null, values);

    public static long LongCount(this IQueryable source, string predicate, Settings settings, params object[] values)
        => LongCount(source, predicate, null, settings, values);

    public static long LongCount(this IQueryable source, string predicate, VarType variables, params object[] values)
        => LongCount(source, predicate, variables, null, values);

    public static long LongCount(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => (long)ExecuteOptionalExpression(source, "LongCount", predicate, string.IsNullOrEmpty(predicate), variables, values, settings)!;
}