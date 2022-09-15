using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfPairOld {
        public ReadOnlyCollection<string> Key { get; }
        public string Value { get; }

        public ConfPairOld(ReadOnlyCollection<string> key, string value) {
            Key = key;
            Value = value;
        }
    }
}
