using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static object First<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return First(source, predicate, null, values);
        }

        public static object First<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return First((IQueryable)source, predicate, variables, values);
        }

        public static object First(this IQueryable source, string predicate, params object[] values) {
            return First(source, predicate, null, values);
        }

        public static object First(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return ExecuteLambda(source, "First", predicate, false, variables, values);
        }

        public static object FirstOrDefault<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return FirstOrDefault(source, predicate, null, values);
        }

        public static object FirstOrDefault<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return FirstOrDefault((IQueryable)source, predicate, variables, values);
        }

        public static object FirstOrDefault(this IQueryable source, string predicate, params object[] values) {
            return FirstOrDefault(source, predicate, null, values);
        }

        public static object FirstOrDefault(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return ExecuteLambda(source, "FirstOrDefault", predicate, false, variables, values);
        }
    }
}
