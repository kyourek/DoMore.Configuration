using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfContent {
<<<<<<< HEAD
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
=======
        public ReadOnlyCollection<ConfPair> Conf { get; }

        public ConfContent(ReadOnlyCollection<ConfPair> conf) {
            Conf = conf ?? throw new ArgumentNullException(nameof(conf));
        }

        public IEnumerable<ConfPair> ByKey(string key) {
            return string.IsNullOrWhiteSpace(key)
                ? Conf
                : Conf.Where(c => c.StartsWith(key));
>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537
        }
    }
}
