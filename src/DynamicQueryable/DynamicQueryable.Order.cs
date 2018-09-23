using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string selector, params object[] values) {
            return OrderBy(source, selector, null, values);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string selector, Dictionary<string, object> variables, params object[] values) {
            return (IQueryable<T>)OrderBy((IQueryable)source, selector, variables, values);
        }

        public static IQueryable OrderBy(this IQueryable source, string selector, params object[] values) {
            return OrderBy(source, selector, null, values);
        }

        public static IQueryable OrderBy(this IQueryable source, string selector, Dictionary<string, object> variables, params object[] values) {
            return HandleLambda(source, "OrderBy", selector, true, variables, values);
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string selector, params object[] values) {
            return OrderByDescending(source, selector, null, values);
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string selector, Dictionary<string, object> variables, params object[] values) {
            return (IQueryable<T>)OrderByDescending((IQueryable)source, selector, variables, values);
        }

        public static IQueryable OrderByDescending(this IQueryable source, string selector, params object[] values) {
            return OrderByDescending(source, selector, null, values);
        }

        public static IQueryable OrderByDescending(this IQueryable source, string selector, Dictionary<string, object> variables, params object[] values) {
            return HandleLambda(source, "OrderByDescending", selector, true, variables, values);
        }

        public static IQueryable<T> ThenBy<T>(this IQueryable<T> source, string selector, params object[] values) {
            return ThenBy(source, selector, null, values);
        }

        public static IQueryable<T> ThenBy<T>(this IQueryable<T> source, string selector, Dictionary<string, object> variables, params object[] values) {
            return (IQueryable<T>)ThenBy((IQueryable)source, selector, variables, values);
        }

        public static IQueryable ThenBy(this IQueryable source, string selector, params object[] values) {
            return ThenBy(source, selector, null, values);
        }

        public static IQueryable ThenBy(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return HandleLambda(source, "ThenBy", selector, true, variables, values);
        }

        public static IQueryable<T> ThenByDescending<T>(this IQueryable<T> source, string selector, params object[] values) {
            return ThenByDescending(source, selector, null, values);
        }

        public static IQueryable<T> ThenByDescending<T>(this IQueryable<T> source, string selector, Dictionary<string, object> variables, params object[] values) {
            return (IQueryable<T>)ThenByDescending((IQueryable)source, selector, variables, values);
        }

        public static IQueryable ThenByDescending(this IQueryable source, string selector, params object[] values) {
            return ThenByDescending(source, selector, null, values);
        }

        public static IQueryable ThenByDescending(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return HandleLambda(source, "ThenByDescending", selector, true, variables, values);
        }
    }
}
