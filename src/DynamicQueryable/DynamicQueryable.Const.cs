using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable Take(this IQueryable source, int count) {
            return HandleConstant(source, "Take", count);
        }

        public static IQueryable Skip(this IQueryable source, int count) {
            return HandleConstant(source, "Skip", count);
        }
    }
}
