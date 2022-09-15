using System;
using System.Text;

namespace Domore.Conf.Future.Text.Parsing.Tokens {
    internal class KeyIndexPartBuilder : TokenBuilder {
        public StringBuilder String { get; } = new StringBuilder();

        public KeyIndexBuilder KeyIndex { get; }

        public KeyIndexPartBuilder(KeyIndexBuilder keyIndex) {
            KeyIndex = keyIndex ?? throw new ArgumentNullException(nameof(keyIndex));
            KeyIndex.Parts.Add(this);
        }

        public override Token Add(string s, ref int i) {
            var c = s[i];
            switch (c) {
                case '\n':
                    return new Invalid();
                case ']':
                    return new KeyIndexBuilder(KeyIndex.KeyPart);
                case ',':
                    return new KeyIndexPartBuilder(KeyIndex);
                default:
                    if (char.IsWhiteSpace(c)) {
                    }
                    else {
                        String.Append(c);
                    }
                    return this;
            }
        }

        public ConfKeyIndexPart KeyIndexPart() {
            return new ConfKeyIndexPart(name: String.ToString());
        }
    }
}
