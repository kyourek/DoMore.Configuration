using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfPair {
        public ReadOnlyCollection<string> Key { get; }
        public string Value { get; }

        public ConfPair(ReadOnlyCollection<string> key, string value) {
            Key = key;
            Value = value;
        }
    }
}
