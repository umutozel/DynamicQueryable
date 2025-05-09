using Jokenizer.Net;
using VarType = System.Collections.Generic.IDictionary<string, object?>;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic;

public static partial class DynamicQueryable {

    public static T First<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => First(source, predicate, null, null, values);

    public static T First<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => First(source, predicate, null, settings, values);

    public static T First<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => First(source, predicate, variables, null, values);

    public static T First<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => (T)First((IQueryable)source, predicate, variables, settings, values);

    public static object First(this IQueryable source, string? predicate = null, params object[] values)
        => First(source, predicate, null, null, values);

    public static object First(this IQueryable source, string predicate, Settings settings, params object[] values)
        => First(source, predicate, null, settings, values);

    public static object First(this IQueryable source, string predicate, VarType variables, params object[] values)
        => First(source, predicate, variables, null, values);

    public static object First(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "First", predicate, string.IsNullOrEmpty(predicate), variables, values, settings)!;

    public static T? FirstOrDefault<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => FirstOrDefault(source, predicate, null, null, values);

    public static T? FirstOrDefault<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => FirstOrDefault(source, predicate, null, settings, values);

    public static T? FirstOrDefault<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => FirstOrDefault(source, predicate, variables, null, values);

    public static T? FirstOrDefault<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => (T?)FirstOrDefault((IQueryable)source, predicate, variables, settings, values);

    public static object? FirstOrDefault(this IQueryable source, string? predicate = null, params object[] values)
        => FirstOrDefault(source, predicate, null, null, values);

    public static object? FirstOrDefault(this IQueryable source, string predicate, Settings settings, params object[] values)
        => FirstOrDefault(source, predicate, null, settings, values);

    public static object? FirstOrDefault(this IQueryable source, string predicate, VarType variables, params object[] values)
        => FirstOrDefault(source, predicate, variables, null, values);

    public static object? FirstOrDefault(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "FirstOrDefault", predicate, string.IsNullOrEmpty(predicate), variables, values, settings);

    public static T Single<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => Single(source, predicate, null, null, values);

    public static T Single<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => Single(source, predicate, null, settings, values);

    public static T Single<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => Single(source, predicate, variables, null, values);

    public static T Single<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => (T)Single((IQueryable)source, predicate, variables, settings, values);

    public static object Single(this IQueryable source, string? predicate = null, params object[] values)
        => Single(source, predicate, null, null, values);

    public static object Single(this IQueryable source, string predicate, Settings settings, params object[] values)
        => Single(source, predicate, null, settings, values);

    public static object Single(this IQueryable source, string predicate, VarType variables, params object[] values)
        => Single(source, predicate, variables, null, values);

    public static object Single(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "Single", predicate, string.IsNullOrEmpty(predicate), variables, values, settings)!;

    public static T? SingleOrDefault<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => SingleOrDefault(source, predicate, null, null, values);

    public static T? SingleOrDefault<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => SingleOrDefault(source, predicate, null, settings, values);

    public static T? SingleOrDefault<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => SingleOrDefault(source, predicate, variables, null, values);

    public static T? SingleOrDefault<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => (T?)SingleOrDefault((IQueryable)source, predicate, variables, settings, values);

    public static object? SingleOrDefault(this IQueryable source, string? predicate = null, params object[] values)
        => SingleOrDefault(source, predicate, null, null, values);

    public static object? SingleOrDefault(this IQueryable source, string predicate, Settings settings, params object[] values)
        => SingleOrDefault(source, predicate, null, settings, values);

    public static object? SingleOrDefault(this IQueryable source, string predicate, VarType variables, params object[] values)
        => SingleOrDefault(source, predicate, variables, null, values);

    public static object? SingleOrDefault(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "SingleOrDefault", predicate, string.IsNullOrEmpty(predicate), variables, values, settings);

    public static T Last<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => Last(source, predicate, null, null, values);

    public static T Last<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => Last(source, predicate, null, settings, values);

    public static T Last<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => Last(source, predicate, variables, null, values);

    public static T Last<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => (T)Last((IQueryable)source, predicate, variables, settings, values);

    public static object Last(this IQueryable source, string? predicate = null, params object[] values)
        => Last(source, predicate, null, null, values);

    public static object Last(this IQueryable source, string predicate, Settings settings, params object[] values)
        => Last(source, predicate, null, settings, values);

    public static object Last(this IQueryable source, string predicate, VarType variables, params object[] values)
        => Last(source, predicate, variables, null, values);

    public static object Last(this IQueryable source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => ExecuteOptionalExpression(source, "Last", predicate, string.IsNullOrEmpty(predicate), variables, values, settings)!;

    public static T? LastOrDefault<T>(this IQueryable<T> source, string? predicate = null, params object[] values)
        => LastOrDefault(source, predicate, null, null, values);

    public static T? LastOrDefault<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => LastOrDefault(source, predicate, null, settings, values);

    public static T? LastOrDefault<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => LastOrDefault(source, predicate, variables, null, values);

    public static T? LastOrDefault<T>(this IQueryable<T> source, string? predicate, VarType? variables, Settings? settings, params object[] values)
        => (T?)LastOrDefault((IQueryable)source, predicate, variables, settings, values);

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