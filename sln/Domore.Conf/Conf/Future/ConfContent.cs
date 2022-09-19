using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfContent {
        public ReadOnlyCollection<ConfPair> Pairs { get; }

        public ConfContent(ReadOnlyCollection<ConfPair> pairs) {
            Pairs = pairs ?? throw new ArgumentNullException(nameof(pairs));
        }

        public IEnumerable<ConfPair> PairsOf(string key) {
            foreach (var pair in Pairs) {
                if (pair.Key.StartsWith(key)) {
                    yield return new ConfPair(pair.Key.Skip(1), pair.Value);
                }
            }
        }
    }
}
