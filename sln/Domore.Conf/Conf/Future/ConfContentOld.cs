using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf.Future {
    public class ConfContentOld : IConf {
        private readonly Dictionary<ConfKeyOld, ConfItem> Set;
        private readonly Dictionary<ConfKeyOld, IList<ConfPairOld>> Pair = new Dictionary<ConfKeyOld, IList<ConfPairOld>>();

        private ReadOnlyCollection<ConfPairOld> Pairs =>
            _Pairs ?? (
            _Pairs = new ReadOnlyCollection<ConfPairOld>(Set.Values.Select(i => i.Pair).ToList()));
        private ReadOnlyCollection<ConfPairOld> _Pairs;

        private IList<ConfPairOld> GetPairs(string key) {
            var k = new ConfKeyOld(key);
            if (Pair.TryGetValue(k, out var value) == false) {
                Pair[k] = value = Set.Values
                    .Where(v => v.Key.StartsWith(k))
                    .Select(v => new ConfPairOld(new ReadOnlyCollection<string>(v.Key.Parts.Skip(k.Parts.Count).ToArray()), v.Value))
                    .ToList();
            }
            return value;
        }

        private ConfContentOld(Dictionary<ConfKeyOld, ConfItem> set) {
            Set = set ?? throw new ArgumentNullException(nameof(set));
        }

        public T Configure<T>(T target, string key) {
            if (null == target) throw new ArgumentNullException(nameof(target));
            var k = key ?? typeof(T).Name;
            var p = k == "" ? Pairs : GetPairs(k);
            new ConfPopulator().Populate(target, this, p);
            return target;
        }

        public IEnumerable<T> Configure<T>(Func<T> factory, string key = null) {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            var k = key ?? typeof(T).Name;
            var p = k == "" ? Pairs : GetPairs(k);
            var i = factory();
            new ConfPopulator().Populate(i, this, p);
            yield return i;
        }

        public static ConfContentOld From(IEnumerable<KeyValuePair<string, string>> pairs) {
            if (null == pairs) throw new ArgumentNullException(nameof(pairs));
            var set = new Dictionary<ConfKeyOld, ConfItem>(new ConfKeyOld.Comparer());
            foreach (var pair in pairs) {
                var key = new ConfKeyOld(pair.Key);
                set[key] = new ConfItem(key, pair.Value);
            }
            return new ConfContentOld(set);
        }
    }
}
