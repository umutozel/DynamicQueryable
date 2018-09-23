using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static class DynamicQueryable {

        public static IQueryable<T> As<T>(this IQueryable source) {
            return (IQueryable<T>)source;
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return Where<T>(source, predicate, null, values);
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return (IQueryable<T>)Where((IQueryable)source, predicate, variables, values);
        }

        public static IQueryable Where(this IQueryable source, string predicate, params object[] parameters) {
            return Where(source, predicate, null, parameters);
        }

        public static IQueryable Where(IQueryable source, string predicate, IDictionary<string, object> variables, params object[] parameters) {
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

        public static IQueryable Select(this IQueryable source, string selector, params object[] values) {
            return Select(source, selector, null, values);
        }

        public static IQueryable Select(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            var lambda = Evaluator.ToLambda(selector, new[] { source.ElementType }, variables, values);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Select",
                    new[] { source.ElementType, lambda.Body.Type },
                    source.Expression,
                    Expression.Quote(lambda)
                )
            );
        }

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
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            var lambda = Evaluator.ToLambda(selector, new[] { source.ElementType }, variables, values);

            // Fix lambda by recreating to be of correct Func<> type in case 
            // the expression parsed to something other than IEnumerable<T>.
            // For instance, a expression evaluating to List<T> would result 
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

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values) {
            return OrderBy(source, ordering, null, values);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, Dictionary<string, object> variables, params object[] values) {
            return (IQueryable<T>)OrderBy((IQueryable)source, ordering, variables, values);
        }

        public static IQueryable OrderBy(this IQueryable source, string ordering, params object[] values) {
            return OrderBy(source, ordering, null, values);
        }

        public static IQueryable OrderBy(this IQueryable source, string ordering, Dictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException("source");
            if (ordering == null) throw new ArgumentNullException("ordering");

            var lambda = Evaluator.ToLambda(ordering, new[] { source.ElementType }, variables, values);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "OrderBy",
                    new[] { source.ElementType, lambda.ReturnType },
                    source.Expression,
                    Expression.Quote(lambda)
                )
            );
        }

        public static IQueryable Take(this IQueryable source, int count) {
            if (source == null) throw new ArgumentNullException("source");

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Take",
                    new[] { source.ElementType },
                    source.Expression,
                    Expression.Constant(count))
                );
        }

        public static IQueryable Skip(this IQueryable source, int count) {
            if (source == null) throw new ArgumentNullException("source");

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Skip",
                    new[] { source.ElementType },
                    source.Expression,
                    Expression.Constant(count)
                )
            );
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, string resultSelector, params object[] values) {
            return GroupBy(source, keySelector, elementSelector, resultSelector, null, values);
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, string resultSelector, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");
            if (elementSelector == null) throw new ArgumentNullException("resultSelector");

            var keyLambda = Evaluator.ToLambda(keySelector, new[] { source.ElementType }, variables, values);
            var elementLambda = Evaluator.ToLambda(elementSelector, new[] { source.ElementType }, variables, values);
            var enumElementType = typeof(IEnumerable<>).MakeGenericType(elementLambda.Body.Type);
            var resultLambda = Evaluator.ToLambda(resultSelector, new[] { keyLambda.Body.Type, enumElementType }, variables, values);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "GroupBy",
                    new[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type, resultLambda.Body.Type },
                    source.Expression,
                    Expression.Quote(keyLambda),
                    Expression.Quote(elementLambda),
                    Expression.Quote(resultLambda)
                )
            );
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector, params object[] values) {
            return GroupBy(source, keySelector, resultSelector, (IDictionary<string, object>)null, values);
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            var keyLambda = Evaluator.ToLambda(keySelector, new[] { source.ElementType }, variables, values);
            var enumSourceType = typeof(IEnumerable<>).MakeGenericType(source.ElementType);
            var resultLambda = Evaluator.ToLambda(resultSelector, new[] { keyLambda.Body.Type, enumSourceType }, variables, values);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "GroupBy",
                    new[] { source.ElementType, keyLambda.Body.Type, resultLambda.Body.Type },
                    source.Expression,
                    Expression.Quote(keyLambda),
                    Expression.Quote(resultLambda)
                )
            );
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, params object[] values) {
            return GroupBy(source, keySelector, (IDictionary<string, object>)null, values);
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");

            var keyLambda = Evaluator.ToLambda(keySelector, new[] { source.ElementType }, variables, values);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "GroupBy",
                    new[] { source.ElementType, keyLambda.Body.Type },
                    source.Expression,
                    Expression.Quote(keyLambda)
                )
            );
        }
    }
}
