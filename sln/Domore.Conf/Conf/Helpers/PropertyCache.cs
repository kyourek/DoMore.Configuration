using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Domore.Conf.Helpers {
    internal class PropertyCache {
        private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> Cache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        public PropertyInfo Get(Type type, string propertyName) {
            if (Cache.TryGetValue(type, out var cache) == false) {
                Cache[type] = cache = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            }
            if (cache.TryGetValue(propertyName, out var propertyInfo) == false) {
                cache[propertyName] = propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            }
            return propertyInfo;
        }
    }
}
