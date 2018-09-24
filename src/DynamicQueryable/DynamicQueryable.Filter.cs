using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return Where<T>(source, predicate, null, values);
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return (IQueryable<T>)Where((IQueryable)source, predicate, variables, values);
        }

        public static IQueryable Where(this IQueryable source, string predicate, params object[] values) {
            return Where(source, predicate, null, values);
        }

        public static IQueryable Where(IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return HandleLambda(source, "Where", predicate, false, variables, values);
        }

        public static IQueryable<T> SkipWhile<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return SkipWhile(source, predicate, null, values);
        }

        public static IQueryable<T> SkipWhile<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return (IQueryable<T>)SkipWhile((IQueryable)source, predicate, variables, values);
        }

        public static IQueryable SkipWhile(this IQueryable source, string predicate, params object[] values) {
            return SkipWhile(source, predicate, null, values);
        }

        public static IQueryable SkipWhile(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return HandleLambda(source, "SkipWhile", predicate, false, variables, values);
        }

        public static IQueryable<T> TakeWhile<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return TakeWhile(source, predicate, null, values);
        }

        public static IQueryable<T> TakeWhile<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return (IQueryable<T>)TakeWhile((IQueryable)source, predicate, variables, values);
        }

        public static IQueryable TakeWhile(this IQueryable source, string predicate, params object[] values) {
            return TakeWhile(source, predicate, null, values);
        }

        public static IQueryable TakeWhile(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return HandleLambda(source, "TakeWhile", predicate, false, variables, values);
        }
    }
}
