using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static class DynamicQueryable {

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return Where<T>(source, predicate, null, values);
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return (IQueryable<T>)WhereImpl(source, predicate, variables, values);
        }

        public static IQueryable Where(this IQueryable source, string predicate, params object[] parameters) {
            return Where(source, predicate, null, parameters);
        }

        public static IQueryable Where(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] parameters) {
            return WhereImpl(source, predicate, variables, parameters);
        }

        public static IQueryable WhereImpl(IQueryable source, string predicate, IDictionary<string, object> variables, params object[] parameters) {
            var types = new[] { source.ElementType };
            var lambda = Evaluator.ToLambda(predicate, types, variables, parameters);
            return source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "Where", types, source.Expression, Expression.Quote(lambda))
            );
        }
    }
}
