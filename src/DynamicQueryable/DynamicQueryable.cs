using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable<T> As<T>(this IQueryable source) {
            return (IQueryable<T>)source;
        }

        public static IQueryable HandleConstant(this IQueryable source, string method, object value) {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    method,
                    new[] { source.ElementType },
                    source.Expression,
                    Expression.Constant(value)
                )
            );
        }

        private static object Execute(this IQueryable source, string method, bool generic) {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable),
                    method,
                    generic ? new[] { source.ElementType } : new Type[] { },
                    source.Expression
                )
            );
        }

        private static object ExecuteSelector(this IQueryable source, string method, string selector, bool generic, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrEmpty(selector))
                return Execute(source, method, generic);

            var types = new[] { source.ElementType };
            var lambda = Evaluator.ToLambda(selector, types, variables, values);

            return source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable),
                    method,
                    generic ? new[] { source.ElementType, lambda.Body.Type } : types,
                    source.Expression,
                    Expression.Quote(lambda)
                )
            );
        }
    }
}
