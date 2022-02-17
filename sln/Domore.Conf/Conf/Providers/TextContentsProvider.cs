using System;
using System.Collections.Generic;
using System.Linq;

namespace Domore.Conf.Providers {
    internal class TextContentsProvider : IConfContentsProvider {
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
                        if (value == "@\"" && separator == '\n') {
                            value = "";
                            for (var k = i + 1; k < count; k++) {
                                var next = items[k].Trim();
                                if (next == "\"") {
                                    var v = value.Trim();
                                    if (v != "") {
                                        yield return new KeyValuePair<string, string>(key, v);
                                    }
                                    i = k + 1;
                                    break;
                                }
                                else {
                                    value = value + Environment.NewLine + next;
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
            contents = contents ?? new KeyValuePair<string, string>[] { };
            return string.Join(Environment.NewLine, contents.Select(item => $"{item.Key} = {item.Value}"));
        }
    }
}
