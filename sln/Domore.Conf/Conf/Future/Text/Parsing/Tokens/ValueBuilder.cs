using System;
using System.Text;

namespace Domore.Conf.Future.Text.Parsing.Tokens {
    internal class ValueBuilder : TokenBuilder {
        private StringBuilder WhiteSpace { get; } = new StringBuilder();

        public StringBuilder String { get; } = new StringBuilder();

        public KeyBuilder Key { get; }

        public ValueBuilder(KeyBuilder key) : base((key ?? throw new ArgumentNullException(nameof(key))).Sep) {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public override Token Add(string s, ref int i) {
            var c = s[i];
            if (c == Sep) return new Complete(Key, String);
            switch (c) {
                case '{':
                    for (var j = i + 1; j < s.Length; j++) {
                        if (s[j] == Sep) {
                            return new ValueContentBuilder(Key);
                        }
                        if (char.IsWhiteSpace(s[j]) == false) {
                            break;
                        }
                    }
                    goto default;
                default:
                    if (char.IsWhiteSpace(c)) {
                        if (String.Length > 0) {
                            WhiteSpace.Append(c);
                        }
                    }
                    else {
                        String.Append(WhiteSpace);
                        String.Append(c);
                        WhiteSpace.Clear();
                    }
                    return this;
            }
        }
    }
}
