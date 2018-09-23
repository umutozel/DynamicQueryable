using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static object Average<T>(this IQueryable<T> source, string selector, params object[] values) {
            return ExecuteSelector(source, "Average", selector, null, values);
        }

        public static object Average<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Average", selector, variables, values);
        }

        public static object Average(this IQueryable source, string selector = null, params object[] values) {
            return ExecuteSelector(source, "Average", selector, null, values);
        }

        public static object Average(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Average", selector, variables, values);
        }

        private static object ExecuteSelector(this IQueryable source, string method, string selector, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrEmpty(selector))
                return Execute(source, method);

            var types = new[] { source.ElementType };
            var lambda = Evaluator.ToLambda(selector, types, variables, values);

            return source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable),
                    method,
                    types,
                    source.Expression,
                    Expression.Quote(lambda)
                )
            );
        }
    }
}
