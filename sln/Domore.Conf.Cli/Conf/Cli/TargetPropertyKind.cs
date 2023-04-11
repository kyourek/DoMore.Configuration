using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Domore.Conf.Cli {
    using Extensions;

    internal static class TargetPropertyKind {
        private static readonly HashSet<Type> Numbers = new HashSet<Type>(new[] { typeof(decimal), typeof(double), typeof(float) });
        private static readonly HashSet<Type> Integers = new HashSet<Type>(new[] { typeof(byte), typeof(sbyte), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(short), typeof(ushort) });

        public static string For(Type type) {
            if (Numbers.Contains(type)) {
                return "num";
            }
            if (Integers.Contains(type)) {
                return "int";
            }
            if (type.IsEnum) {
                return string.Join("|", CliType.GetEnumDisplay(type).Select(pair => pair.Value.ToLowerInvariant()));
            }
            if (typeof(IList).IsAssignableFrom(type)) {
                var itemType = ConfType.GetItemType(type);
                var itemKind = For(itemType);
                return itemKind == null
                    ? ","
                    : ",<" + itemKind + ">";
            }
            return null;
        }
    }
}
