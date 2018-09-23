using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable<T> As<T>(this IQueryable source) {
            return (IQueryable<T>)source;
        }

        public static IQueryable Take(this IQueryable source, int count) {
            return HandleConstant(source, "Take", count);
        }

        public static IQueryable Skip(this IQueryable source, int count) {
            return HandleConstant(source, "Skip", count);
        }

        public static IQueryable HandleConstant(this IQueryable source, string method, int count) {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    method,
                    new[] { source.ElementType },
                    source.Expression,
                    Expression.Constant(count)
                )
            );
        }

        private static object Execute(this IQueryable source, string method) {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable),
                    method,
                    new Type[] { },
                    source.Expression
                )
            );
        }
    }
}
