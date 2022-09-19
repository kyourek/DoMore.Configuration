using System.Collections.Generic;
using System.Text;

namespace Domore.Conf.Future.Text.Parsing.Tokens {
    internal class ValueContentBuilder : ValueBuilder {
        private StringBuilder Line = new StringBuilder();
        private List<StringBuilder> Lines { get; } = new List<StringBuilder>();
        private StringBuilder WhiteSpace { get; } = new StringBuilder();

        public ValueContentBuilder(KeyBuilder key) : base(key) {
        }

        public override Token Add(string s, ref int i) {
            var c = s[i];
            if (c == Sep) {
                if (Line.Length > 0) {
                    Lines.Add(Line);
                }
                Line = new StringBuilder();
                WhiteSpace.Clear();
                return this;
            }
            switch (c) {
                case '}':
                    if (Line.Length == 0) {
                        for (var j = i + 1; j < s.Length; j++) {
                            if (s[j] == Sep) {
                                if (Lines.Count > 0) {
                                    String.Append(string.Join(Sep.ToString(), Lines));
                                    return new Complete(Key, String);
                                }
                                return new KeyBuilder(Sep);
                            }
                            if (char.IsWhiteSpace(s[j]) == false) {
                                break;
                            }
                        }
                    }
                    goto default;
                default:
                    if (char.IsWhiteSpace(c)) {
                        if (Line.Length > 0) {
                            WhiteSpace.Append(c);
                        }
                    }
                    else {
                        Line.Append(WhiteSpace);
                        Line.Append(c);
                        WhiteSpace.Clear();
                    }
                    return this;
            }
        }
    }
}
