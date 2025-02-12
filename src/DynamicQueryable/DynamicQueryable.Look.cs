using System.Collections.Generic;
using Jokenizer.Net;
using VarType = System.Collections.Generic.IDictionary<string, object?>;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic;

public static partial class DynamicQueryable {

    public static bool All(this IQueryable source, string? predicate = null, params object[] values)
        => All(source, predicate, null, null, values);

    public static bool All(this IQueryable source, string predicate, Settings settings, params object[] values)
        => All(source, predicate, null, settings, values);

    public static bool All(this IQueryable source, string predicate, VarType variables, params object[] values)
        => All(source, predicate, variables, null, values);

    public static bool All(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => (bool)ExecuteLambda(source, "All", predicate, false, variables, values, settings)!;

    public static bool Any(this IQueryable source, string? predicate = null, params object[] values)
        => Any(source, predicate, null, null, values);

    public static bool Any(this IQueryable source, string predicate, Settings settings, params object[] values)
        => Any(source, predicate, null, settings, values);

    public static bool Any(this IQueryable source, string predicate, VarType variables, params object[] values)
        => Any(source, predicate, variables, null, values);

    public static bool Any(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => (bool)ExecuteOptionalExpression(source, "Any", predicate, string.IsNullOrEmpty(predicate), variables, values, settings)!;

    public static bool Contains(this IQueryable source, object item)
        => (bool)ExecuteConstant(source, "Contains", item)!;

    public static bool SequenceEqual(this IQueryable source, IEnumerable<object> items)
        => (bool)ExecuteConstant(source, "SequenceEqual", items)!;
}