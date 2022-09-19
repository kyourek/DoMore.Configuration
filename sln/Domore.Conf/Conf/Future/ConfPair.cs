using System;

namespace Domore.Conf.Future {
    internal class ConfPair {
        public ConfKey Key { get; }
        public string Value { get; }

        public ConfPair(ConfKey key, string value) {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value;
        }

        public bool StartsWith(string key) {
            return Key.StartsWith(key);
        }
    }
}
