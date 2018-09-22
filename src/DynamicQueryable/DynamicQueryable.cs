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

        private static IQueryable WhereImpl(IQueryable source, string predicate, IDictionary<string, object> variables, params object[] parameters) {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            var types = new[] { source.ElementType };
            var lambda = Evaluator.ToLambda(predicate, types, variables, parameters);
            return source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), 
                "Where", 
                types, 
                source.Expression, 
                Expression.Quote(lambda))
            );
        }

        public static IQueryable<object> Select(this IQueryable source, string selector, params object[] values) {
            return Select(source, selector, null, values);
        }

        public static IQueryable<object> Select(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            
            var lambda = Evaluator.ToLambda(selector, new[] { source.ElementType }, variables, values);
            return (IQueryable<object>)source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), 
                    "Select", 
                    new[] { source.ElementType, lambda.Body.Type }, 
                    source.Expression, 
                    Expression.Quote(lambda)
                )
            );
        }
    }
}
