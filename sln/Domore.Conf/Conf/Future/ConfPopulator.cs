using System;
using System.Collections.Generic;
using System.Linq;

namespace Domore.Conf.Future {
    internal class ConfPopulator {
        private readonly Dictionary<Type, Dictionary<string, ConfProperty>> ConfPropertyCache = new Dictionary<Type, Dictionary<string, ConfProperty>>();

        private readonly ConfPropertyCache PropertyCache = new ConfPropertyCache();
        private readonly ConfValueConverter ConverterDefault = new ConfValueConverter();
        private readonly ConfValueConverterCache ConverterCache = new ConfValueConverterCache();

        private object Convert(string value, ConfObjectProperty property, IConf conf) {
            if (null == property) throw new ArgumentNullException(nameof(property));
            var converterType = property.Attribute?.Converter;
            var converter = converterType == null ? ConverterDefault : ConverterCache.Get(converterType);
            var converted = converter.Convert(value, new ConfValueConverterState(property.Object, property.PropertyInfo, conf));
            return converted;
        }

        private void Populate(IList<string> key, string value, object target, IConf conf) {
            if (key != null && key.Count > 0) {
                var property = new ConfObjectProperty(target, key[0], PropertyCache);
                if (property.Exists) {
                    if (property.Populate) {
                        switch (key.Count) {
                            case 1:
                                var item = property.Item;
                                if (item != null && item.Exists) {
                                    if (item.Populate) {
                                        item.PropertyValue = Convert(value, item, conf);
                                    }
                                }
                                else {
                                    property.PropertyValue = Convert(value, property, conf);
                                }
                                break;
                            default:
                                var keys = key.Skip(1).ToArray();
                                var populator = new ConfPopulator();
                                var propertyValue = property.PropertyValue;
                                if (propertyValue is null) {
                                    propertyValue = property.PropertyValue = Activator.CreateInstance(property.PropertyType);
                                }
                                populator.Populate(keys, value, propertyValue, conf);
                                break;
                        }
                    }
                }
            }
        }

        public void Populate(object target, IConf conf, IEnumerable<ConfPairOld> pairs) {
            if (null == pairs) throw new ArgumentNullException(nameof(pairs));
            foreach (var pair in pairs) {
                Populate(pair.Key, pair.Value, target, conf);
            }
        }

        public void Populate(object target, IConf conf, IEnumerable<ConfPair> content) {
            if (null == target) throw new ArgumentNullException(nameof(target));
            var targetType = target.GetType();
            if (ConfPropertyCache.TryGetValue(targetType, out var targetPropCache) == false) {
                ConfPropertyCache[targetType] = targetPropCache = new Dictionary<string, ConfProperty>(StringComparer.OrdinalIgnoreCase);
            }
            foreach (var pair in content) {
                var key = pair.Key;
                foreach (var part in key.Parts) {
                    var targetPropName = part.Name;
                    if (targetPropCache.TryGetValue(targetPropName, out var property) == false) {
                        targetPropCache[targetPropName] = property = new ConfProperty(targetPropName, targetType);
                    }
                    if (property.Exists) {
                        if (property.Populate) {

                        }
                    }
                }
            }
        }
    }
}
