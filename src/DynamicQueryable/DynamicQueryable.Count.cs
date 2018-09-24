using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static int Count(this IQueryable source, string predicate = null, params object[] values) {
            return Count(source, predicate, null, values);
        }

        public static int Count(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return (int)ExecuteOptionalExpression(source, "Count", predicate, string.IsNullOrEmpty(predicate), variables, values);
        }

        public static long LongCount(this IQueryable source, string predicate = null, params object[] values) {
            return LongCount(source, predicate, null, values);
        }

        public static long LongCount(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return (long)ExecuteOptionalExpression(source, "LongCount", predicate, string.IsNullOrEmpty(predicate), variables, values);
        }
    }
}
