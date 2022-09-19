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

        public KeyPartBuilder(KeyBuilder key) : base((key ?? throw new ArgumentNullException(nameof(key))).Sep) {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Key.Parts.Add(this);
        }

        public override Token Add(string s, ref int i) {
            var c = Next(s, ref i);
            if (c == null) return null;
            if (c == Sep) return new KeyBuilder(Sep);
            switch (c) {
                case '=':
                    return new ValueBuilder(Key);
                case '[':
                    return new KeyIndexBuilder(this);
                case '.':
                    return new KeyPartBuilder(Key);
                default:
                    String.Append(c);
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
