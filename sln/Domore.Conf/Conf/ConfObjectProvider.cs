using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domore.Conf {
    internal class ConfObjectProvider {
        private IEnumerable<KeyValuePair<string, string>> ListConfContents(IList list, string key) {
            if (null == list) throw new ArgumentNullException(nameof(list));
            if (key == null) {
                var listType = list.GetType();
                var listArgs = listType.GetGenericArguments();
                if (listArgs.Length == 1) {
                    key = listArgs[0].Name;
                }
            }

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
            if (key == null) {
                var dictType = dictionary.GetType();
                var dictArgs = dictType.GetGenericArguments();
                if (dictArgs.Length == 2) {
                    key = dictArgs[1].Name;
                }
            }

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
            string k(string s) => key == "" ? s : string.Join(".", key, s);
            foreach (var property in properties) {
                if (property.CanRead) {
                    var parameters = property.GetIndexParameters();
                    if (parameters.Length == 0) {
                        var propertyValue = property.GetValue(source, null);
                        if (propertyValue != null) {
                            var propertyValueType = propertyValue.GetType();
                            if (propertyValueType.IsValueType || propertyValueType == typeof(string)) {
                                if (property.CanWrite) {
                                    var pairKey = k(property.Name);
                                    var pairValue = Convert.ToString(propertyValue);
                                    if (pairValue.Contains("\n")) {
                                        pairValue = string.Join(Environment.NewLine, "{", pairValue, "}");
                                    }
                                    var pair = new KeyValuePair<string, string>(pairKey, pairValue);
                                    yield return pair;
                                }
                            }
                            else {
                                var cc = GetConfContents(propertyValue, k(property.Name));
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

        public string GetConfText(object source, string key = null, bool? multiline = null) {
            var equals = multiline == false ? "=" : " = ";
            var separator = multiline == false ? ";" : Environment.NewLine;
            var confContents = GetConfContents(source, key);
            return string.Join(separator, confContents
                .Select(pair => string.Join(equals, pair.Key, pair.Value)));
        }
    }
}
