using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static bool All(this IQueryable source, string predicate = null, params object[] values) {
            return All(source, predicate, null, values);
        }

        public static bool All(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return (bool)ExecuteLambda(source, "All", predicate, false, variables, values);
        }

        public static bool Any(this IQueryable source, string predicate = null, params object[] values) {
            return Any(source, predicate, null, values);
        }

        public static bool Any(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return (bool)ExecuteOptionalExpression(source, "Any", predicate, string.IsNullOrEmpty(predicate), variables, values);
        }

        public static bool Contains(this IQueryable source, object item) {
            return (bool)ExecuteConstant(source, "Contains", true, item);
        }
    }
}
