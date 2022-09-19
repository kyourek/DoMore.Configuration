using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf.Future.Text.Parsing.Tokens {
    internal class KeyIndexBuilder : TokenBuilder {
        public List<KeyIndexPartBuilder> Parts { get; } = new List<KeyIndexPartBuilder>();

        public KeyPartBuilder KeyPart { get; }

        public KeyIndexBuilder(KeyPartBuilder keyPart) : base((keyPart ?? throw new ArgumentNullException(nameof(keyPart))).Sep) {
            KeyPart = keyPart ?? throw new ArgumentNullException(nameof(keyPart));
            KeyPart.Indices.Add(this);
        }

        public override Token Add(string s, ref int i) {
            var c = Next(s, ref i);
            if (c == null) return null;
            if (c == Sep) return new KeyBuilder(Sep);
            switch (c) {
                default:
                    i--;
                    return new KeyIndexPartBuilder(this);
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
