using System;
using System.Text;

namespace Domore.Conf.Future.Text.Parsing.Tokens {
    internal class Complete : Token {
        public KeyBuilder Key { get; }
        public StringBuilder Value { get; }

        public Complete(KeyBuilder key, StringBuilder value) {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ConfPair Pair() {
            return new ConfPair(
                key: Key.Key(),
                value: Value.ToString());
        }
    }
}
