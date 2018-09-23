using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable<T> SelectMany<T>(this IQueryable source, string selector, params object[] values) {
            return SelectMany<T>(source, selector, null, values);
        }

        public static IQueryable<T> SelectMany<T>(this IQueryable source, string selector, Dictionary<string, object> variables, params object[] values) {
            return (IQueryable<T>)SelectMany((IQueryable)source, selector, null, values);
        }

        public static IQueryable SelectMany(this IQueryable source, string selector, params object[] values) {
            return SelectMany(source, selector, null, values);
        }

        public static IQueryable SelectMany(this IQueryable source, string selector, Dictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var lambda = Evaluator.ToLambda(selector, new[] { source.ElementType }, variables, values);

            // Fix lambda by recreating to be of correct Func<> type in case 
            // the expression parsed to something other than IEnumerable<T>.
            // For instance, an expression evaluating to List<T> would result 
            // in a lambda of type Func<T, List<T>> when we need one of type
            // an Func<T, IEnumerable<T> in order to call SelectMany().
            var inputType = source.Expression.Type.GetGenericArguments()[0];
            var resultType = lambda.Body.Type.GetGenericArguments()[0];
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(resultType);
            var delegateType = typeof(Func<,>).MakeGenericType(inputType, enumerableType);
            lambda = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "SelectMany",
                    new[] { source.ElementType, resultType },
                    source.Expression,
                    Expression.Quote(lambda)
                )
            );
        }
    }
}
