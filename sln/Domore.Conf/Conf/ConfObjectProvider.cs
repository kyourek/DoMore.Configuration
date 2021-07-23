using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domore.Conf {
    internal class ConfObjectProvider {
        public IEnumerable<KeyValuePair<string, string>> GetConfContents(object source, string key = null) {
            if (null == source) throw new ArgumentNullException(nameof(source));

            var type = source.GetType();
            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(property => property.Name);

            key = key ?? type.Name;
            foreach (var property in properties) {
                if (property.CanRead) {
                    var parameters = property.GetIndexParameters();
                    if (parameters.Length == 0) {
                        var propertyValue = property.GetValue(source, null);
                        if (propertyValue != null) {
                            var propertyValueType = propertyValue.GetType();
                            if (propertyValueType.IsValueType || propertyValueType == typeof(string)) {
                                if (property.CanWrite) {
                                    var pairKey = string.Join(".", key, property.Name);
                                    var pairValue = Convert.ToString(propertyValue);
                                    var pair = new KeyValuePair<string, string>(pairKey, pairValue);
                                    yield return pair;
                                }
                            }
                            else {
                                var cc = GetConfContents(propertyValue, string.Join(".", key, property.Name));
                                foreach (var item in cc) {
                                    yield return item;
                                }
                            }
                        }
                    }
                }
            }
        }

        public string GetConfText(object source, string key = null) {
            var confContents = GetConfContents(source, key);
            return string.Join(Environment.NewLine,
                confContents.Select(pair => string.Join(" = ",
                    pair.Key, pair.Value)));
        }
    }
}
