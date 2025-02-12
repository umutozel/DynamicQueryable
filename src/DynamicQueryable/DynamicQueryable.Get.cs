using Jokenizer.Net;
using VarType = System.Collections.Generic.IDictionary<string, object?>;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic;

public static partial class DynamicQueryable {

    public static object First<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => First(source, predicate, null, null, values);

    public static object First<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => First(source, predicate, null, settings, values);

    public static object First<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => First(source, predicate, variables, null, values);

    public static object First<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => First((IQueryable)source, predicate, variables, settings, values);

    public static object First(this IQueryable source, string? predicate = null, params object[] values)
        => First(source, predicate, null, null, values);

    public static object First(this IQueryable source, string predicate, Settings settings, params object[] values)
        => First(source, predicate, null, settings, values);

    public static object First(this IQueryable source, string predicate, VarType variables, params object[] values)
        => First(source, predicate, variables, null, values);

    public static object First(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "First", predicate, string.IsNullOrEmpty(predicate), variables, values, settings)!;

    public static object? FirstOrDefault<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => FirstOrDefault(source, predicate, null, null, values);

    public static object? FirstOrDefault<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => FirstOrDefault(source, predicate, null, settings, values);

    public static object? FirstOrDefault<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => FirstOrDefault(source, predicate, variables, null, values);

    public static object? FirstOrDefault<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => FirstOrDefault((IQueryable)source, predicate, variables, settings, values);

    public static object? FirstOrDefault(this IQueryable source, string? predicate = null, params object[] values)
        => FirstOrDefault(source, predicate, null, null, values);

    public static object? FirstOrDefault(this IQueryable source, string predicate, Settings settings, params object[] values)
        => FirstOrDefault(source, predicate, null, settings, values);

    public static object? FirstOrDefault(this IQueryable source, string predicate, VarType variables, params object[] values)
        => FirstOrDefault(source, predicate, variables, null, values);

    public static object? FirstOrDefault(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "FirstOrDefault", predicate, string.IsNullOrEmpty(predicate), variables, values, settings);

    public static object Single<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => Single(source, predicate, null, null, values);

    public static object Single<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => Single(source, predicate, null, settings, values);

    public static object Single<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => Single(source, predicate, variables, null, values);

    public static object Single<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => Single((IQueryable)source, predicate, variables, settings, values);

    public static object Single(this IQueryable source, string? predicate = null, params object[] values)
        => Single(source, predicate, null, null, values);

    public static object Single(this IQueryable source, string predicate, Settings settings, params object[] values)
        => Single(source, predicate, null, settings, values);

    public static object Single(this IQueryable source, string predicate, VarType variables, params object[] values)
        => Single(source, predicate, variables, null, values);

    public static object Single(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "Single", predicate, string.IsNullOrEmpty(predicate), variables, values, settings)!;

    public static object? SingleOrDefault<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => SingleOrDefault(source, predicate, null, null, values);

    public static object? SingleOrDefault<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => SingleOrDefault(source, predicate, null, settings, values);

    public static object? SingleOrDefault<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => SingleOrDefault(source, predicate, variables, null, values);

    public static object? SingleOrDefault<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => SingleOrDefault((IQueryable)source, predicate, variables, settings, values);

    public static object? SingleOrDefault(this IQueryable source, string? predicate = null, params object[] values)
        => SingleOrDefault(source, predicate, null, null, values);

    public static object? SingleOrDefault(this IQueryable source, string predicate, Settings settings, params object[] values)
        => SingleOrDefault(source, predicate, null, settings, values);

    public static object? SingleOrDefault(this IQueryable source, string predicate, VarType variables, params object[] values)
        => SingleOrDefault(source, predicate, variables, null, values);

    public static object? SingleOrDefault(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "SingleOrDefault", predicate, string.IsNullOrEmpty(predicate), variables, values, settings);

    public static object Last<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => Last(source, predicate, null, null, values);

    public static object Last<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => Last(source, predicate, null, settings, values);

    public static object Last<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => Last(source, predicate, variables, null, values);

    public static object Last<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => Last((IQueryable)source, predicate, variables, settings, values);

    public static object Last(this IQueryable source, string? predicate = null, params object[] values)
        => Last(source, predicate, null, null, values);

    public static object Last(this IQueryable source, string predicate, Settings settings, params object[] values)
        => Last(source, predicate, null, settings, values);

    public static object Last(this IQueryable source, string predicate, VarType variables, params object[] values)
        => Last(source, predicate, variables, null, values);

    public static object Last(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "Last", predicate, string.IsNullOrEmpty(predicate), variables, values, settings)!;

    public static object? LastOrDefault<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => LastOrDefault(source, predicate, null, null, values);

    public static object? LastOrDefault<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => LastOrDefault(source, predicate, null, settings, values);

    public static object? LastOrDefault<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => LastOrDefault(source, predicate, variables, null, values);

    public static object? LastOrDefault<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => LastOrDefault((IQueryable)source, predicate, variables, settings, values);

    public static object? LastOrDefault(this IQueryable source, string? predicate = null, params object[] values)
        => LastOrDefault(source, predicate, null, null, values);

    public static object? LastOrDefault(this IQueryable source, string predicate, Settings settings, params object[] values)
        => LastOrDefault(source, predicate, null, settings, values);

    public static object? LastOrDefault(this IQueryable source, string predicate, VarType variables, params object[] values)
        => LastOrDefault(source, predicate, variables, null, values);

    public static object? LastOrDefault(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "LastOrDefault", predicate, string.IsNullOrEmpty(predicate), variables, values, settings);

    public static object ElementAt(this IQueryable source, int index)
        => ExecuteConstant(source, "ElementAt", index)!;

    public static object? ElementAtOrDefault(this IQueryable source, int index)
        => ExecuteConstant(source, "ElementAtOrDefault", index);
}