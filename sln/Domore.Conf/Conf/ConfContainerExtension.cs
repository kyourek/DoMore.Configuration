using System;
using System.Collections.Generic;

namespace Domore.Conf {
    public static class ConfContainerExtension {
        private static IConfBlock ValidateBlock(IConfContainer confContainer) {
            if (null == confContainer) throw new ArgumentNullException(nameof(confContainer));

            var block = confContainer.Block;
            if (block == null) throw new ArgumentException(paramName: nameof(confContainer), message: $"{nameof(confContainer.Block)} is null.");

            return block;
        }

        public static T Configure<T>(this IConfContainer confContainer, T obj, string key = null) {
            return ValidateBlock(confContainer).Configure(obj, key);
        }

        public static IEnumerable<T> Configure<T>(this IConfContainer confContainer, Func<T> factory, string key = null, IEqualityComparer<string> comparer = null) {
            return ValidateBlock(confContainer).Configure(factory, key, comparer);
        }

        public static IEnumerable<KeyValuePair<string, T>> Configure<T>(this IConfContainer confContainer, Func<string, T> factory, string key = null, IEqualityComparer<string> comparer = null) {
            return ValidateBlock(confContainer).Configure(factory, key, comparer);
        }
    }
}
