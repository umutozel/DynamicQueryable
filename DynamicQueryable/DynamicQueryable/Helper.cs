using System;
using System.Reflection;

namespace DynamicQueryable {

    internal static class Helper {

        internal static TypeCode GetTypeCode(Type type) {
#if NET_STANDARD
            if (type == null) return TypeCode.Empty;
            if (type == typeof(Boolean)) return TypeCode.Boolean;
            if (type == typeof(Byte)) return TypeCode.Byte;
            if (type == typeof(Char)) return TypeCode.Char;
            if (type == typeof(DateTime)) return TypeCode.DateTime;
            if (type == typeof(Decimal)) return TypeCode.Decimal;
            if (type == typeof(Double)) return TypeCode.Double;
            if (type == typeof(Int16)) return TypeCode.Int16;
            if (type == typeof(Int32)) return TypeCode.Int32;
            if (type == typeof(Int64)) return TypeCode.Int64;
            if (type == typeof(SByte)) return TypeCode.SByte;
            if (type == typeof(Single)) return TypeCode.Single;
            if (type == typeof(String)) return TypeCode.String;
            if (type == typeof(UInt16)) return TypeCode.UInt16;
            if (type == typeof(UInt32)) return TypeCode.UInt32;
            if (type == typeof(UInt64)) return TypeCode.UInt64;

            return TypeCode.Object;
#else
            return Type.GetTypeCode(type);
#endif
        }

        internal static MemberInfo[] FindMembers(this Type type, MemberTypes memberType, BindingFlags bindingAttr,
                                                 Func<MemberInfo, object, bool> func, object filterCriteria) {
            return null;
        }
    }
}
