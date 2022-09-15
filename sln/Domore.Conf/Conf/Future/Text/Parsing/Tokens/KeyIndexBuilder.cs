using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf.Future.Text.Parsing.Tokens {
    internal class KeyIndexBuilder : TokenBuilder {
        public List<KeyIndexPartBuilder> Parts { get; } = new List<KeyIndexPartBuilder>();

        public KeyPartBuilder KeyPart { get; }

        public KeyIndexBuilder(KeyPartBuilder keyPart) {
            KeyPart = keyPart ?? throw new ArgumentNullException(nameof(keyPart));
            KeyPart.Indices.Add(this);
        }

        public override Token Add(string s, ref int i) {
            var c = s[i];
            switch (c) {
                case '[':
                    return new KeyIndexPartBuilder(this);
                case '.':
                    return new KeyPartBuilder(KeyPart.Key);
                default:
                    return new Invalid();
            }
        }

        public ConfKeyIndex KeyIndex() {
            return new ConfKeyIndex(
                parts: new ReadOnlyCollection<ConfKeyIndexPart>(
                    Parts
                        .Select(p => p.KeyIndexPart())
                        .ToList()));
        }
    }
}
