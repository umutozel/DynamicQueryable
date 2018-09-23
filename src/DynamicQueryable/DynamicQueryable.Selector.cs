using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable Select(this IQueryable source, string selector, params object[] values) {
            return Select(source, selector, null, values);
        }

        public static IQueryable Select(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return HandleSelector(source, "Select", selector, variables, values);
        }

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
            return HandleSelector(source, "OrderBy", selector, variables, values);
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
            return HandleSelector(source, "OrderByDescending", selector, variables, values);
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
            return HandleSelector(source, "ThenBy", selector, variables, values);
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
            return HandleSelector(source, "ThenByDescending", selector, variables, values);
        }

        public static IQueryable HandleSelector(this IQueryable source, string method, string selector, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var lambda = Evaluator.ToLambda(selector, new[] { source.ElementType }, variables, values);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    method,
                    new[] { source.ElementType, lambda.ReturnType },
                    source.Expression,
                    Expression.Quote(lambda)
                )
            );
        }
    }
}
