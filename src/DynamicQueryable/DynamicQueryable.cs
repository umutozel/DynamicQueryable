using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable<T> As<T>(this IQueryable source) {
            return (IQueryable<T>)source;
        }

        public static IQueryable Take(this IQueryable source, int count) {
            if (source == null) throw new ArgumentNullException(nameof(source));

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
            if (source == null) throw new ArgumentNullException(nameof(source));

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
    }
}
