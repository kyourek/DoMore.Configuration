using System.Collections.Generic;

namespace Domore.Conf.Future {
    internal class ConfContent {
        public IEnumerable<IConfPair> Pairs { get; }

        public ConfContent(params IConfPair[] pairs) {
            Pairs = new List<IConfPair>(pairs);
        }

        public IEnumerable<IConfPair> PairsOf(string key) {
            foreach (var pair in Pairs) {
                if (pair.Key.StartsWith(key)) {
                    yield return new ConfPair(pair.Key.Skip(), pair.Value);
                }
            }
        }
    }
}
