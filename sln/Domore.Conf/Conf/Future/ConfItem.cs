namespace Domore.Conf.Future {
    internal class ConfItem {
        public ConfPair Pair =>
            _Pair ?? (
            _Pair = new ConfPair(Key.Parts, Value));
        private ConfPair _Pair;

        public ConfKey Key { get; }
        public string Value { get; }

        public ConfItem(ConfKey key, string value) {
            Key = key;
            Value = value;
        }
    }
}
