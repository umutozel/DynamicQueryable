using System.Collections.Generic;
using System.Linq.Expressions;
using Jokenizer.Net;

namespace System.Linq.Dynamic {

    public static partial class DynamicQueryable {

        public static bool SequenceEqual(this IQueryable source, IEnumerable<object> items) {
            return (bool)ExecuteConstant(source, "SequenceEqual", true, items);
        }
    }
}
