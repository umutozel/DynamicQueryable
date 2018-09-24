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

        public static IQueryable Distinct(this IQueryable source) {
            return Handle(source, "Distinct");
        }

        public static IQueryable Reverse(this IQueryable source) {
            return Handle(source, "Reverse");
        }

        public static IQueryable DefaultIfEmpty(this IQueryable source) {
            return Handle(source, "DefaultIfEmpty");
        }

        public static IQueryable DefaultIfEmpty(this IQueryable source, object defaultValue) {
            return HandleConstant(source, "DefaultIfEmpty", defaultValue);
        }
    }
}
