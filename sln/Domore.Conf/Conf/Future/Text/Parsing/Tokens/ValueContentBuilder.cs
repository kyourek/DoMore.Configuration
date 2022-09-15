using System;
using System.Text;

namespace Domore.Conf.Future.Text.Parsing.Tokens {
    internal class ValueContentBuilder : TokenBuilder {
        public StringBuilder String { get; } = new StringBuilder();

        public KeyBuilder Key { get; }

        public ValueContentBuilder(KeyBuilder key) {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public override Token Add(string s, ref int i) {
            var c = s[i];
            switch (c) {
                case '}':
                    for (var j = i + 1; j < s.Length; j++) {
                        if (s[j] == '\n') {
                            return new Complete(Key, String);
                        }
                        if (char.IsWhiteSpace(s[j]) == false) {
                            break;
                        }
                    }
                    goto default;
                default:
                    if (char.IsWhiteSpace(c)) {
                        if (String.Length > 0) {
                            String.Append(c);
                        }
                    }
                    else {
                        String.Append(c);
                    }
                    return this;
            }
        }
    }
}
