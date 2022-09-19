using System;
using System.Text;

namespace Domore.Conf.Future.Text.Parsing.Tokens {
    internal class KeyIndexPartBuilder : TokenBuilder {
        public StringBuilder String { get; } = new StringBuilder();

        public KeyIndexBuilder KeyIndex { get; }

        public KeyIndexPartBuilder(KeyIndexBuilder keyIndex) : base((keyIndex ?? throw new ArgumentNullException(nameof(keyIndex))).Sep) {
            KeyIndex = keyIndex ?? throw new ArgumentNullException(nameof(keyIndex));
            KeyIndex.Parts.Add(this);
        }

        public override Token Add(string s, ref int i) {
            var c = Next(s, ref i);
            if (c == null) return null;
            if (c == Sep) return new KeyBuilder(Sep);
            switch (c) {
                case ']':
                    return KeyIndex.KeyPart;
                case ',':
                    return new KeyIndexPartBuilder(KeyIndex);
                default:
                    String.Append(c);
                    return this;
            }
        }

        public ConfKeyIndexPart KeyIndexPart() {
            return new ConfKeyIndexPart(name: String.ToString());
        }
    }
}
