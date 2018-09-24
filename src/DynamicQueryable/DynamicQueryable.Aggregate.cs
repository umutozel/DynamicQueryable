using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static object Average(this IQueryable source, string selector = null, params object[] values) {
            return Average(source, selector, null, values);
        }

        public static object Average(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "Average", selector, false, variables, values);
        }

        public static object Sum(this IQueryable source, string selector = null, params object[] values) {
            return Sum(source, selector, null, values);
        }

        public static object Sum(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "Sum", selector, false, variables, values);
        }

        public static object Max(this IQueryable source, string selector = null, params object[] values) {
            return Max(source, selector, null, values);
        }

        public static object Max(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "Max", selector, true, variables, values);
        }

        public static object Min(this IQueryable source, string selector = null, params object[] values) {
            return Min(source, selector, null, values);
        }

        public static object Min(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "Min", selector, true, variables, values);
        }

        public static object Aggregate(this IQueryable source, string func, params object[] values) {
            return Aggregate(source, func, null, values);
        }

        public static object Aggregate(this IQueryable source, string func, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(func)) throw new ArgumentNullException(nameof(func));

            var funcLambda = Evaluator.ToLambda(func, new[] { source.ElementType, source.ElementType }, variables, values);

            return source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable),
                    "Aggregate",
                    new[] { source.ElementType },
                    source.Expression,
                    Expression.Quote(funcLambda)
                )
            );
        }

        public static object Aggregate(this IQueryable source, object seed, string func, params object[] values) {
            return Aggregate(source, seed, func, (IDictionary<string, object>)null, values);
        }

        public static object Aggregate(this IQueryable source, object seed, string func, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (seed == null) throw new ArgumentNullException(nameof(seed));
            if (string.IsNullOrWhiteSpace(func)) throw new ArgumentNullException(nameof(func));

            var funcLambda = Evaluator.ToLambda(func, new[] { seed.GetType(), source.ElementType }, variables, values);

            return source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable),
                    "Aggregate",
                    new[] { source.ElementType, seed.GetType() },
                    source.Expression,
                    Expression.Constant(seed),
                    Expression.Quote(funcLambda)
                )
            );
        }

        public static object Aggregate(this IQueryable source, object seed, string func, string selector, params object[] values) {
            return Aggregate(source, seed, func, selector, null, values);
        }

        public static object Aggregate(this IQueryable source, object seed, string func, string selector, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (seed == null) throw new ArgumentNullException(nameof(seed));
            if (string.IsNullOrWhiteSpace(func)) throw new ArgumentNullException(nameof(func));
            if (string.IsNullOrWhiteSpace(selector)) throw new ArgumentNullException(nameof(func));

            var funcLambda = Evaluator.ToLambda(func, new[] { seed.GetType(), source.ElementType }, variables, values);
            var selectorLambda = Evaluator.ToLambda(selector, new[] { seed.GetType() }, variables, values);

            return source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable),
                    "Aggregate",
                    new[] { source.ElementType, seed.GetType(), selectorLambda.Body.Type },
                    source.Expression,
                    Expression.Constant(seed),
                    Expression.Quote(funcLambda),
                    Expression.Quote(selectorLambda)
                )
            );
        }
    }
}
