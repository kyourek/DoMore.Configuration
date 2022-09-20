using System;
using System.Text;

namespace Domore.Conf.Future.Text.Parsing.Tokens {
    internal abstract class ValueBuilder : TokenBuilder, IConfValue {
        protected StringBuilder String { get; } = new StringBuilder();

        protected override string Create() {
            return String.ToString();
        }

        public KeyBuilder Key { get; }

        public ValueBuilder(KeyBuilder key) : base((key ?? throw new ArgumentNullException(nameof(key))).Sep) {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }
}
