using System;
using System.Collections.Generic;
using System.Linq;

namespace Domore.Conf.Cli {
    internal static class TargetPropertyKind {
        private static readonly HashSet<Type> Numbers = new HashSet<Type>(new[] { typeof(decimal), typeof(double), typeof(float) });
        private static readonly HashSet<Type> Integers = new HashSet<Type>(new[] { typeof(byte), typeof(sbyte), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(short), typeof(ushort) });

        public static string For(Type type) {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (Numbers.Contains(type)) {
                return "num";
            }
            if (Integers.Contains(type)) {
                return "int";
            }
            if (type.IsEnum) {
                return string.Join("|", type.GetEnumNames().Select(n => n.ToLowerInvariant()));
            }
            return null;
        }
    }
}
