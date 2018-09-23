using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable Select(this IQueryable source, string selector, params object[] values) {
            return Select(source, selector, null, values);
        }

        public static IQueryable Select(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return HandleLambda(source, "Select", selector, true, variables, values);
        }
    }
}
