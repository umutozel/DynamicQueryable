using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static object Average<T>(this IQueryable<T> source, string selector, params object[] values) {
            return ExecuteSelector(source, "Average", selector, false, null, values);
        }

        public static object Average<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Average", selector, false, variables, values);
        }

        public static object Average(this IQueryable source, string selector = null, params object[] values) {
            return ExecuteSelector(source, "Average", selector, false, null, values);
        }

        public static object Average(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Average", selector, false, variables, values);
        }

        public static object Sum<T>(this IQueryable<T> source, string selector, params object[] values) {
            return ExecuteSelector(source, "Sum", selector, false, null, values);
        }

        public static object Sum<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Sum", selector, false, variables, values);
        }

        public static object Sum(this IQueryable source, string selector = null, params object[] values) {
            return ExecuteSelector(source, "Sum", selector, false, null, values);
        }

        public static object Sum(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Sum", selector, false, variables, values);
        }

        public static object Max<T>(this IQueryable<T> source, string selector, params object[] values) {
            return ExecuteSelector(source, "Max", selector, true, null, values);
        }

        public static object Max<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Max", selector, true, variables, values);
        }

        public static object Max(this IQueryable source, string selector = null, params object[] values) {
            return ExecuteSelector(source, "Max", selector, true, null, values);
        }

        public static object Max(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Max", selector, true, variables, values);
        }

        public static object Min<T>(this IQueryable<T> source, string selector, params object[] values) {
            return ExecuteSelector(source, "Min", selector, true, null, values);
        }

        public static object Min<T>(this IQueryable<T> source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Min", selector, true, variables, values);
        }

        public static object Min(this IQueryable source, string selector = null, params object[] values) {
            return ExecuteSelector(source, "Min", selector, true, null, values);
        }

        public static object Min(this IQueryable source, string selector, IDictionary<string, object> variables, params object[] values) {
            return ExecuteSelector(source, "Min", selector, true, variables, values);
        }
    }
}
