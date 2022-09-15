using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Domore.Conf.Future.Text.Parsing.Tokens {
    internal class KeyPartBuilder : TokenBuilder {
        public StringBuilder String { get; } = new StringBuilder();
        public List<KeyIndexBuilder> Indices { get; } = new List<KeyIndexBuilder>();

        public KeyBuilder Key { get; }

        public KeyPartBuilder(KeyBuilder key) {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Key.Parts.Add(this);
        }

        public override Token Add(string s, ref int i) {
            var c = s[i];
            switch (c) {
                case '\n':
                    return new Invalid();
                case '=':
                    return new ValueBuilder(Key);
                case '[':
                    i--;
                    return new KeyIndexBuilder(this);
                case '.':
                    return new KeyPartBuilder(Key);
                default:
                    if (char.IsWhiteSpace(c)) {
                    }
                    else {
                        String.Append(c);
                    }
                    return this;
            }
        }

        public ConfKeyPart KeyPart() {
            return new ConfKeyPart(
                name: String.ToString(),
                indices: new ReadOnlyCollection<ConfKeyIndex>(
                    Indices
                        .Select(i => i.KeyIndex())
                        .ToList()));
        }
    }
}
