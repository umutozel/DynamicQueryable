using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static object Average<T>(this IQueryable<T> source, string selector, params object[] values) {
            return ExecuteSelector(source, "Average", selector, null, values);
        }

        public static object Average<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Average", selector, variables, values);
        }

        public static object Average(this IQueryable source, string selector = null, params object[] values) {
            return ExecuteSelector(source, "Average", selector, null, values);
        }

        public static object Average(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Average", selector, variables, values);
        }

        public static object Sum<T>(this IQueryable<T> source, string selector, params object[] values) {
            return ExecuteSelector(source, "Sum", selector, null, values);
        }

        public static object Sum<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Sum", selector, variables, values);
        }

        public static object Sum(this IQueryable source, string selector = null, params object[] values) {
            return ExecuteSelector(source, "Sum", selector, null, values);
        }

        public static object Sum(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Sum", selector, variables, values);
        }

        public static object Max<T>(this IQueryable<T> source, string selector, params object[] values) {
            return ExecuteGenericSelector(source, "Max", selector, null, values);
        }

        public static object Max<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteGenericSelector(source, "Max", selector, variables, values);
        }

        public static object Max(this IQueryable source, string selector = null, params object[] values) {
            return ExecuteGenericSelector(source, "Max", selector, null, values);
        }

        public static object Max(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteGenericSelector(source, "Max", selector, variables, values);
        }
    }
}
