using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable Except<T>(this IQueryable source, IEnumerable<T> items) {
            return HandleConstant(source, "Except", items);
        }

        public static IQueryable Intersect<T>(this IQueryable source, IEnumerable<T> items) {
            return HandleConstant(source, "Intersect", items);
        }

        public static IQueryable Union<T>(this IQueryable source, IEnumerable<T> items) {
            return HandleConstant(source, "Union", items);
        }

        public static IQueryable Concat<T>(this IQueryable source, IEnumerable<T> items) {
            return HandleConstant(source, "Concat", items);
        }
    }
}
