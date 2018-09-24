using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static object Average<T>(this IQueryable<T> source, string selector, params object[] values) {
            return Average(source, selector, null, values);
        }

        public static object Average<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return Average((IQueryable)source, selector, variables, values);
        }

        public static object Average(this IQueryable source, string selector = null, params object[] values) {
            return Average(source, selector, null, values);
        }

        public static object Average(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "Average", selector, false, variables, values);
        }

        public static object Sum<T>(this IQueryable<T> source, string selector, params object[] values) {
            return Sum(source, selector, null, values);
        }

        public static object Sum<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return Sum((IQueryable)source, selector, variables, values);
        }

        public static object Sum(this IQueryable source, string selector = null, params object[] values) {
            return Sum(source, selector, null, values);
        }

        public static object Sum(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "Sum", selector, false, variables, values);
        }

        public static object Max<T>(this IQueryable<T> source, string selector, params object[] values) {
            return Max(source, selector, null, values);
        }

        public static object Max<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return Max((IQueryable)source, selector, variables, values);
        }

        public static object Max(this IQueryable source, string selector = null, params object[] values) {
            return Max(source, selector, null, values);
        }

        public static object Max(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "Max", selector, true, variables, values);
        }

        public static object Min<T>(this IQueryable<T> source, string selector, params object[] values) {
            return Min(source, selector, null, values);
        }

        public static object Min<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return Min((IQueryable)source, selector, variables, values);
        }

        public static object Min(this IQueryable source, string selector = null, params object[] values) {
            return Min(source, selector, null, values);
        }

        public static object Min(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "Min", selector, true, variables, values);
        }

        public static int Count<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return Count(source, predicate, null, values);
        }

        public static int Count<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return Count((IQueryable)source, predicate, variables, values);
        }

        public static int Count(this IQueryable source, string predicate = null, params object[] values) {
            return Count(source, predicate, null, values);
        }

        public static int Count(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return (int)ExecuteOptionalExpression(source, "Count", predicate, string.IsNullOrEmpty(predicate), variables, values);
        }

        public static long LongCount<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return LongCount(source, predicate, null, values);
        }

        public static long LongCount<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return LongCount((IQueryable)source, predicate, variables, values);
        }

        public static long LongCount(this IQueryable source, string predicate = null, params object[] values) {
            return LongCount(source, predicate, null, values);
        }

        public static long LongCount(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return (long)ExecuteOptionalExpression(source, "LongCount", predicate, string.IsNullOrEmpty(predicate), variables, values);
        }
    }
}
