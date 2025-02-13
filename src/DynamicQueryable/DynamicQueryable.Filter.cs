using Jokenizer.Net;
using VarType = System.Collections.Generic.IDictionary<string, object?>;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic;

public static partial class DynamicQueryable {

    public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values)
        => Where(source, predicate, null, null, values);

    public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => Where(source, predicate, null, settings, values);

    public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => Where(source, predicate, variables, null, values);

    public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, VarType? variables, Settings? settings, params object[] values)
        => (IQueryable<T>)Where((IQueryable)source, predicate, variables, settings, values);

    public static IQueryable Where(this IQueryable source, string predicate, params object[] values)
        => Where(source, predicate, null, null, values);

    public static IQueryable Where(this IQueryable source, string predicate, Settings settings, params object[] values)
        => Where(source, predicate, null, settings, values);

    public static IQueryable Where(this IQueryable source, string predicate, VarType variables, params object[] values)
        => Where(source, predicate, variables, null, values);

    public static IQueryable Where(this IQueryable source, string predicate, VarType? variables, Settings? settings, params object[] values)
        => HandleLambda(source, "Where", predicate, false, variables, values, settings);

    public static IQueryable<T> SkipWhile<T>(this IQueryable<T> source, string predicate, params object[] values)
        => SkipWhile(source, predicate, null, null, values);

    public static IQueryable<T> SkipWhile<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => SkipWhile(source, predicate, null, settings, values);

    public static IQueryable<T> SkipWhile<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => SkipWhile(source, predicate, variables, null, values);

    public static IQueryable<T> SkipWhile<T>(this IQueryable<T> source, string predicate, VarType? variables, Settings? settings, params object[] values)
        => (IQueryable<T>)SkipWhile((IQueryable)source, predicate, variables, settings, values);

    public static IQueryable SkipWhile(this IQueryable source, string predicate, params object[] values)
        => SkipWhile(source, predicate, null, null, values);

    public static IQueryable SkipWhile(this IQueryable source, string predicate, Settings settings, params object[] values)
        => SkipWhile(source, predicate, null, settings, values);

    public static IQueryable SkipWhile(this IQueryable source, string predicate, VarType variables, params object[] values)
        => SkipWhile(source, predicate, variables, null, values);

    public static IQueryable SkipWhile(this IQueryable source, string predicate, VarType? variables, Settings? settings, params object[] values)
        => HandleLambda(source, "SkipWhile", predicate, false, variables, values, settings);

    public static IQueryable<T> TakeWhile<T>(this IQueryable<T> source, string predicate, params object[] values)
        => TakeWhile(source, predicate, null, null, values);

    public static IQueryable<T> TakeWhile<T>(this IQueryable<T> source, string predicate, Settings settings, params object[] values)
        => TakeWhile(source, predicate, null, settings, values);

    public static IQueryable<T> TakeWhile<T>(this IQueryable<T> source, string predicate, VarType variables, params object[] values)
        => TakeWhile(source, predicate, variables, null, values);

    public static IQueryable<T> TakeWhile<T>(this IQueryable<T> source, string predicate, VarType? variables, Settings? settings, params object[] values)
        => (IQueryable<T>)TakeWhile((IQueryable)source, predicate, variables, settings, values);

    public static IQueryable TakeWhile(this IQueryable source, string predicate, params object[] values)
        => TakeWhile(source, predicate, null, null, values);

    public static IQueryable TakeWhile(this IQueryable source, string predicate, Settings settings, params object[] values)
        => TakeWhile(source, predicate, null, settings, values);

    public static IQueryable TakeWhile(this IQueryable source, string predicate, VarType variables, params object[] values)
        => TakeWhile(source, predicate, variables, null, values);

    public static IQueryable TakeWhile(this IQueryable source, string predicate, VarType? variables, Settings? settings, params object[] values)
        => HandleLambda(source, "TakeWhile", predicate, false, variables, values, settings);
}