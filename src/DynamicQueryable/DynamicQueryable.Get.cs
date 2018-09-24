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

        public static object First(this IQueryable source, string predicate = null, params object[] values) {
            return First(source, predicate, null, values);
        }

        public static object First(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "First", predicate, string.IsNullOrEmpty(predicate), variables, values);
        }

        public static object FirstOrDefault<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return FirstOrDefault(source, predicate, null, values);
        }

        public static object FirstOrDefault<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return FirstOrDefault((IQueryable)source, predicate, variables, values);
        }

        public static object FirstOrDefault(this IQueryable source, string predicate = null, params object[] values) {
            return FirstOrDefault(source, predicate, null, values);
        }

        public static object FirstOrDefault(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "FirstOrDefault", predicate, string.IsNullOrEmpty(predicate), variables, values);
        }

        public static object Single<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return Single(source, predicate, null, values);
        }

        public static object Single<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return Single((IQueryable)source, predicate, variables, values);
        }

        public static object Single(this IQueryable source, string predicate = null, params object[] values) {
            return Single(source, predicate, null, values);
        }

        public static object Single(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "Single", predicate, string.IsNullOrEmpty(predicate), variables, values);
        }

        public static object SingleOrDefault<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return SingleOrDefault(source, predicate, null, values);
        }

        public static object SingleOrDefault<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return SingleOrDefault((IQueryable)source, predicate, variables, values);
        }

        public static object SingleOrDefault(this IQueryable source, string predicate = null, params object[] values) {
            return SingleOrDefault(source, predicate, null, values);
        }

        public static object SingleOrDefault(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "SingleOrDefault", predicate, string.IsNullOrEmpty(predicate), variables, values);
        }

        public static object Last<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return Last(source, predicate, null, values);
        }

        public static object Last<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return Last((IQueryable)source, predicate, variables, values);
        }

        public static object Last(this IQueryable source, string predicate = null, params object[] values) {
            return Last(source, predicate, null, values);
        }

        public static object Last(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "Last", predicate, string.IsNullOrEmpty(predicate), variables, values);
        }

        public static object LastOrDefault<T>(this IQueryable<T> source, string predicate, params object[] values) {
            return LastOrDefault(source, predicate, null, values);
        }

        public static object LastOrDefault<T>(this IQueryable<T> source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return LastOrDefault((IQueryable)source, predicate, variables, values);
        }

        public static object LastOrDefault(this IQueryable source, string predicate = null, params object[] values) {
            return LastOrDefault(source, predicate, null, values);
        }

        public static object LastOrDefault(this IQueryable source, string predicate, IDictionary<string, object> variables, params object[] values) {
            return ExecuteOptionalExpression(source, "LastOrDefault", predicate, string.IsNullOrEmpty(predicate), variables, values);
        }

        public static object ElementAt(this IQueryable source, int index) {
            return ExecuteConstant(source, "ElementAt", index);
        }

        public static object ElementAtOrDefault(this IQueryable source, int index) {
            return ExecuteConstant(source, "ElementAtOrDefault", index);
        }
    }
}
