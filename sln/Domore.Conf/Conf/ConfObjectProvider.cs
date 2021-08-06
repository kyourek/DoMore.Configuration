using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domore.Conf {
    internal class ConfObjectProvider {
        private IEnumerable<KeyValuePair<string, string>> ListConfContents(IList list, string key) {
            if (null == list) throw new ArgumentNullException(nameof(list));
            for (var i = 0; i < list.Count; i++) {
                var k = $"{key}[{i}]";
                var v = list[i];
                if (v != null) {
                    var vType = v.GetType();
                    if (vType.IsValueType || vType == typeof(string)) {
                        yield return new KeyValuePair<string, string>(
                            key: k,
                            value: $"{v}");
                    }
                    else {
                        foreach (var kvp in GetConfContents(v, k)) {
                            yield return kvp;
                        }
                    }
                }
            }
        }

        private IEnumerable<KeyValuePair<string, string>> DictionaryConfContents(IDictionary dictionary, string key) {
            if (null == dictionary) throw new ArgumentNullException(nameof(dictionary));
            var dKeys = dictionary.Keys;
            if (dKeys != null) {
                foreach (var dKey in dKeys) {
                    var k = $"{key}[{dKey}]";
                    var v = dictionary[dKey];
                    if (v != null) {
                        var vType = v.GetType();
                        if (vType.IsValueType || vType == typeof(string)) {
                            yield return new KeyValuePair<string, string>(
                                key: k,
                                value: $"{v}");
                        }
                        else {
                            foreach (var kvp in GetConfContents(v, k)) {
                                yield return kvp;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<KeyValuePair<string, string>> DefaultConfContents(object source, string key) {
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


        public IEnumerable<KeyValuePair<string, string>> GetConfContents(object source, string key = null) {
            if (null == source) throw new ArgumentNullException(nameof(source));

            var list = source as IList;
            if (list != null) {
                return ListConfContents(list, key);
            }

            var dictionary = source as IDictionary;
            if (dictionary != null) {
                return DictionaryConfContents(dictionary, key);
            }

            return DefaultConfContents(source, key);
        }

        public string GetConfText(object source, string key = null) {
            var confContents = GetConfContents(source, key);
            return string.Join(Environment.NewLine,
                confContents.Select(pair => string.Join(" = ",
                    pair.Key, pair.Value)));
        }
    }
}
