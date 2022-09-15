using System;

namespace Domore.Conf.Future {
    internal class ConfPair {
        public ConfKey Key { get; }
        public string Value { get; }

        public ConfPair(ConfKey key, string value) {
            Key = key;
            Value = value;
        }

        public void Populate(object target) {
            if (null == target) throw new ArgumentNullException(nameof(target));
            var type = target.GetType();
        }
    }
}
