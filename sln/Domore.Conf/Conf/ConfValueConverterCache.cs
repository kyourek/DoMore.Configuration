using System;
using System.Collections.Generic;

namespace Domore.Conf {
    internal sealed class ConfValueConverterCache {
        private readonly ConfValueConverter Default = new ConfValueConverter();
        private readonly Dictionary<Type, ConfValueConverter> Cache = new Dictionary<Type, ConfValueConverter>();

        private static ConfValueConverter Create(Type type) {
            return (ConfValueConverter)Activator.CreateInstance(type);
        }

        public ConfValueConverter Get(ConfAttribute attribute) {
            if (attribute == null) {
                return Default;
            }
            var @internal = attribute.ConverterInstance;
            if (@internal != null) {
                return @internal;
            }
            var type = attribute.Converter;
            lock (Cache) {
                if (Cache.TryGetValue(type, out var value) == false) {
                    Cache[type] = value = Create(type);
                }
                return value;
            }
        }
    }
}
