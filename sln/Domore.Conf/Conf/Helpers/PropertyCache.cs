using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domore.Conf.Helpers {
    using Extensions;

    internal class PropertyCache {
        private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> Cache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        private PropertyInfo Create(Type type, string name) {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == name) throw new ArgumentNullException(nameof(name));

            var f = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
            var pi = type.GetProperty(name, f);
            if (pi != null) return pi;

            return type
                .GetProperties(f)
                .Select(p => new {
                    PropertyInfo = p,
                    ConfAttribute = p.GetConfAttribute()
                })
                .Where(i => i.ConfAttribute != null)
                .Where(i => i.ConfAttribute.Names.Any(n => name.Equals(n, StringComparison.OrdinalIgnoreCase)))
                .Select(i => i.PropertyInfo)
                .FirstOrDefault();
        }

        public PropertyInfo Get(Type type, string name) {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (Cache.TryGetValue(type, out var cache) == false) {
                Cache[type] = cache = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            }
            if (cache.TryGetValue(name, out var propertyInfo) == false) {
                cache[name] = propertyInfo = Create(type, name);
            }
            return propertyInfo;
        }
    }
}
