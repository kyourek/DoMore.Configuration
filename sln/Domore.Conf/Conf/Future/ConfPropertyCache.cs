using System;
using System.Collections.Generic;

namespace Domore.Conf.Future {
    internal class ConfPropertyCache {
        private readonly Dictionary<Type, Dictionary<string, ConfProperty>> Cache = new Dictionary<Type, Dictionary<string, ConfProperty>>();

        public StringComparer StringComparer { get; } = StringComparer.OrdinalIgnoreCase;

        public ConfProperty Get(Type type, string name) {
            if (Cache.TryGetValue(type, out var cache) == false) {
                Cache[type] = cache = new Dictionary<string, ConfProperty>(StringComparer);
            }
            if (cache.TryGetValue(name, out var property) == false) {
                cache[name] = property = new ConfProperty(name, type);
            }
            return property;
        }
    }
}
