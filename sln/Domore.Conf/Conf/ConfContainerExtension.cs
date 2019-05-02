using System;

namespace Domore.Conf {
    public static class ConfContainerExtension {
        public static T Configure<T>(this IConfContainer confContainer, T obj, string key = null) {
            if (null == confContainer) throw new ArgumentNullException(nameof(confContainer));

            var block = confContainer.Block;
            if (block == null) throw new ArgumentException(paramName: nameof(confContainer), message: $"{nameof(confContainer.Block)} is null.");

            return block.Configure(obj, key);
        }
    }
}
