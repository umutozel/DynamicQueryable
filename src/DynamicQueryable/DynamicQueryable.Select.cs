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
