using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable<T> As<T>(this IQueryable source) {
            return (IQueryable<T>)source;
        }

        private static Expression CreateLambda(IQueryable source, string method, string expression, bool generic, IDictionary<string, object> variables, params object[] values) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var types = new[] { source.ElementType };
            var lambda = Evaluator.ToLambda(expression, types, variables, values);

            return Expression.Call(
                typeof(Queryable),
                method,
                generic ? new[] { source.ElementType, lambda.Body.Type } : types,
                source.Expression,
                Expression.Quote(lambda)
            );
        }

        private static Expression CreateExpression(IQueryable source, string method, bool generic, params Expression[] expressions) {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return Expression.Call(
                typeof(Queryable),
                method,
                generic ? new[] { source.ElementType } : new Type[0],
                new[] { source.Expression }.Concat(expressions).ToArray()
            );
        }

        private static IQueryable HandleConstant(IQueryable source, string method, object value) {
            return source.Provider.CreateQuery(CreateExpression(source, method, true, Expression.Constant(value)));
        }

        private static IQueryable HandleLambda(IQueryable source, string method, string expression, bool generic, IDictionary<string, object> variables, object[] values) {
            return source.Provider.CreateQuery(CreateLambda(source, method, expression, generic, variables, values));
        }

        private static object Execute(IQueryable source, string method, bool generic) {
            return source.Provider.Execute(CreateExpression(source, method, generic));
        }

        private static object ExecuteLambda(IQueryable source, string method, string expression, bool generic, IDictionary<string, object> variables, params object[] values) {
            if (string.IsNullOrEmpty(expression))
                return Execute(source, method, generic);

            return source.Provider.Execute(CreateLambda(source, method, expression, generic, variables, values));
        }
    }
}
