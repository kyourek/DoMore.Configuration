using System.Collections.Generic;
using System.Text;

namespace Domore.Conf.Cli {
    internal struct Token {
        private Token(string key, string value) {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public string Value { get; }

        public static IEnumerable<Token> Parse(string line) {
            var s = line?.Trim() ?? "";
            if (s == "") {
                yield break;
            }
            var k = new StringBuilder();
            var v = default(StringBuilder);
            var b = k;
            var q = default(char?);
            foreach (var c in s) {
                if (c == '\'' || c == '\"') {
                    if (q == c) {
                        q = null;
                    }
                    else {
                        if (q == null) {
                            q = c;
                        }
                        else {
                            b.Append(c);
                        }
                    }
                }
                else {
                    if (c == '=') {
                        if (q.HasValue) {
                            b.Append(c);
                        }
                        else {
                            b = v = v ?? new StringBuilder();
                        }
                    }
                    else {
                        if (char.IsWhiteSpace(c)) {
                            if (q == null) {
                                yield return new Token(k.ToString(), v?.ToString());
                                k = new StringBuilder();
                                v = null;
                                b = k;
                            }
                            else {
                                b.Append(c);
                            }
                        }
                        else {
                            b.Append(c);
                        }
                    }
                }
            }
            yield return new Token(k.ToString(), v?.ToString());
        }
    }
}
