using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable Except(this IQueryable source, IEnumerable<object> items) {
            return HandleConstant(source, "Except", items);
        }

        public static IQueryable Intersect(this IQueryable source, IEnumerable<object> items) {
            return HandleConstant(source, "Intersect", items);
        }

        public static IQueryable Union(this IQueryable source, IEnumerable<object> items) {
            return HandleConstant(source, "Union", items);
        }

        public static IQueryable Concat(this IQueryable source, IEnumerable<object> items) {
            return HandleConstant(source, "Concat", items);
        }
    }
}
