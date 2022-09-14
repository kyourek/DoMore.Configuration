using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf.Future {
    public class ConfContent : IConf {
        private readonly Dictionary<ConfKey, ConfItem> Set;
        private readonly Dictionary<ConfKey, IList<ConfPair>> Pair = new Dictionary<ConfKey, IList<ConfPair>>();

        private ReadOnlyCollection<ConfPair> Pairs =>
            _Pairs ?? (
            _Pairs = new ReadOnlyCollection<ConfPair>(Set.Values.Select(i => i.Pair).ToList()));
        private ReadOnlyCollection<ConfPair> _Pairs;

        private IList<ConfPair> GetPairs(string key) {
            var k = new ConfKey(key);
            if (Pair.TryGetValue(k, out var value) == false) {
                Pair[k] = value = Set.Values
                    .Where(v => v.Key.StartsWith(k))
                    .Select(v => new ConfPair(new ReadOnlyCollection<string>(v.Key.Parts.Skip(k.Parts.Count).ToArray()), v.Value))
                    .ToList();
            }
            return value;
        }

        private ConfContent(Dictionary<ConfKey, ConfItem> set) {
            Set = set ?? throw new ArgumentNullException(nameof(set));
        }

        public T Configure<T>(T target, string key) {
            if (null == target) throw new ArgumentNullException(nameof(target));
            var k = key ?? target.GetType().Name;
            var p = k == "" ? Pairs : GetPairs(k);
            new ConfPopulator().Populate(target, this, p);
            return target;
        }

        public static ConfContent From(IEnumerable<KeyValuePair<string, string>> pairs) {
            if (null == pairs) throw new ArgumentNullException(nameof(pairs));
            var set = new Dictionary<ConfKey, ConfItem>(new ConfKey.Comparer());
            foreach (var pair in pairs) {
                var key = new ConfKey(pair.Key);
                set[key] = new ConfItem(key, pair.Value);
            }
            return new ConfContent(set);
        }
    }
}
