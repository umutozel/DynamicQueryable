using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable Join(this IQueryable outer, IQueryable inner, string outerKeySelector, string innerKeySelector, string resultSelector, params object[] values) {
            return Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null, values);
        }

        public static IQueryable Join(this IQueryable outer, IQueryable inner, string outerKeySelector, string innerKeySelector, string resultSelector, IDictionary<string, object> variables, params object[] values) {
            if (outer == null) throw new ArgumentNullException(nameof(outer));
            if (inner == null) throw new ArgumentNullException(nameof(inner));
            if (string.IsNullOrWhiteSpace(outerKeySelector)) throw new ArgumentNullException(nameof(outerKeySelector));
            if (string.IsNullOrWhiteSpace(innerKeySelector)) throw new ArgumentNullException(nameof(innerKeySelector));
            if (string.IsNullOrWhiteSpace(resultSelector)) throw new ArgumentNullException(nameof(resultSelector));

            var outerKeyLambda = Evaluator.ToLambda(outerKeySelector, new[] { outer.ElementType }, variables, values);
            var innerKeyLambda = Evaluator.ToLambda(innerKeySelector, new[] { inner.ElementType }, variables, values);
            var resultLambda = Evaluator.ToLambda(resultSelector, new[] { outer.ElementType, inner.ElementType }, variables, values);

            return outer.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Join",
                    new[] { outer.ElementType, inner.ElementType, outerKeyLambda.Body.Type, resultLambda.Body.Type },
                    outer.Expression,
                    Expression.Constant(inner),
                    Expression.Quote(outerKeyLambda),
                    Expression.Quote(innerKeyLambda),
                    Expression.Quote(resultLambda)
                )
            );
        }
    }
}
