using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf.Future.Text.Parsing.Tokens {
    internal class KeyBuilder : TokenBuilder {
        public List<KeyPartBuilder> Parts { get; } = new List<KeyPartBuilder>();

        public KeyBuilder(char sep) : base(sep) {
        }

        public override Token Add(string s, ref int i) {
            var c = Next(s, ref i);
            if (c == null) return null;
            if (c == Sep) return new KeyBuilder(Sep);
            switch (c) {
                default:
                    i--;
                    return new KeyPartBuilder(this);
            }
        }

        public ConfKey Key() {
            return new ConfKey(
                parts: new ReadOnlyCollection<ConfKeyPart>(
                    Parts
                        .Select(p => p.KeyPart())
                        .ToList()));
        }
    }
}
