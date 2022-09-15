namespace Domore.Conf.Future {
    internal class ConfItem {
        public ConfPairOld Pair =>
            _Pair ?? (
            _Pair = new ConfPairOld(Key.Parts, Value));
        private ConfPairOld _Pair;

        public ConfKeyOld Key { get; }
        public string Value { get; }

        public ConfItem(ConfKeyOld key, string value) {
            Key = key;
            Value = value;
        }
    }
}
