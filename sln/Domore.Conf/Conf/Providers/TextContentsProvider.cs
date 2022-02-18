using System;
using System.Collections.Generic;
using System.Linq;

namespace Domore.Conf.Providers {
    using TextContents;

    internal class TextContentsProvider : IConfContentsProvider {
        public static string Multiline(string s) {
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
    }
}
