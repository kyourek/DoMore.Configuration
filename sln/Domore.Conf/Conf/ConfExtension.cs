using System;

namespace Domore.Conf {
    public static class ConfExtension {
        public static T Configure<T>(this IConf conf, T obj, string key = null) {
            if (null == conf) throw new ArgumentNullException(nameof(conf));

            var container = conf.Container;
            if (container == null) throw new ArgumentException(paramName: nameof(conf), message: $"{nameof(conf.Container)} is null.");

            return container.Configure(obj, key);
        }
    }
}
