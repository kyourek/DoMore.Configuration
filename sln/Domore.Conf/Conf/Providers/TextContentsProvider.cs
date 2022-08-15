using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domore.Conf.Providers {
    using Extensions;
    using TextContents;

    internal class TextContentsProvider : IConfContentsProvider {
        private static string Multiline(string s) {
            if (s != null) {
                if (s.Contains('\n')) {
                    s = string.Join(Environment.NewLine, "{", s, "}");
                }
            }
            return s;
        }

        public virtual IEnumerable<KeyValuePair<string, string>> GetConfContents(object content) {
            var contents = content?.ToString()?.Trim() ?? "";
            var separator = contents.Contains('\n') ? '\n' : ';';
            var items = contents.Split(separator).ToList();
            var count = items.Count;
            for (var i = 0; i < count; i++) {
                var item = items[i];
                var length = item.Length;
                for (var j = 0; j < length; j++) {
                    if (item[j] == '=') {
                        var key = item.Substring(0, j).Trim();
                        var value = j == length - 1 ? "" : item.Substring(j + 1).Trim();
                        if (value == "") break;
                        if (value == "{" && separator == '\n') {
                            value = "";
                            for (var k = i + 1; k < count; k++) {
                                var next = items[k];
                                if (next.IsCharAndWhiteSpace('}')) {
                                    if (value.IsWhiteSpace() == false) {
                                        value = value
                                            .Replace("\r", "")
                                            .Replace("\n", Environment.NewLine);
                                        yield return new KeyValuePair<string, string>(key, value);
                                    }
                                    i = k;
                                    break;
                                }
                                else {
                                    value = value == ""
                                        ? next
                                        : (value + Environment.NewLine + next);
                                }
                            }
                        }
                        else {
                            yield return new KeyValuePair<string, string>(key, value);
                        }
                        break;
                    }
                }
            }
        }

        public virtual string GetConfContent(IEnumerable<KeyValuePair<string, string>> contents) {
            return contents == null
                ? string.Empty
                : string.Join(Environment.NewLine, contents
                    .Select(item => item.Key + " = " + Multiline(item.Value)));
        }

        public class Reverse {
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
                                value: Multiline($"{v}"));
                        }
                        else {
                            foreach (var kvp in ConfContents(v, k)) {
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
                                    value: Multiline($"{v}"));
                            }
                            else {
                                foreach (var kvp in ConfContents(v, k)) {
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
                        var confAttr = property.GetConfAttribute();
                        if (confAttr == null || confAttr.Ignore == false) {
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
                                                pairValue = Multiline(pairValue);
                                            }
                                            var pair = new KeyValuePair<string, string>(pairKey, pairValue);
                                            yield return pair;
                                        }
                                    }
                                    else {
                                        var cc = ConfContents(propertyValue, k(property.Name));
                                        foreach (var item in cc) {
                                            yield return item;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public IEnumerable<KeyValuePair<string, string>> ConfContents(object source, string key = null) {
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

            public string ConfText(object source, string key = null, bool? multiline = null) {
                var equals = multiline == false ? "=" : " = ";
                var separator = multiline == false ? ";" : Environment.NewLine;
                var confContents = ConfContents(source, key);
                return string.Join(separator, confContents
                    .Select(pair => string.Join(equals, pair.Key, pair.Value)));
            }
        }
    }
}
