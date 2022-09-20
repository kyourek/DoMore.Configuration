using System.Text;

namespace Domore.Conf.Text.Parsing.Tokens {
    internal sealed class MultilineValueBuilder : ValueBuilder {
        private readonly StringBuilder Line = new StringBuilder();

        public MultilineValueBuilder(KeyBuilder key) : base(key) {
        }

        public override Token Build(string s, ref int i) {
            var c = s[i];
            if (c == Sep) {
                if (Line.Length > 0) {
                    var closing = false;
                    for (var j = 0; j < Line.Length; j++) {
                        var l = Line[j];
                        if (l == '}') {
                            if (closing) {
                                closing = false;
                                break;
                            }
                            closing = true;
                        }
                        else {
                            if (char.IsWhiteSpace(l) == false) {
                                closing = false;
                                break;
                            }
                        }
                    }
                    if (closing) {
                        if (String.Length > 0) {
                            String.Append(c);
                            var whitespace = true;
                            for (var k = 0; k < String.Length; k++) {
                                if (char.IsWhiteSpace(String[k]) == false) {
                                    whitespace = false;
                                    break;
                                }
                            }
                            if (whitespace == false) {
                                return new Complete(Key, this);
                            }
                        }
                        return new KeyBuilder(Sep);
                    }
                }
                String.Append(Line);
                Line.Clear();
            }
            Line.Append(c);
            return this;
        }
    }
}
