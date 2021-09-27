using System;
using System.Collections.Generic;
using System.Linq;

namespace Domore.Conf.Helpers {
    internal static class Collect {
        public static Dictionary<string, string> Pair(string key, IEqualityComparer<string> comparer, IConfBlock conf) {
            if (null == conf) throw new ArgumentNullException(nameof(conf));

            var normK = Key.Normalize(key);
            var count = conf.ItemCount();
            var items = new Dictionary<string, string>(comparer);
            for (var i = 0; i < count; i++) {
                var item = conf.Item(i);
                var itemKey = item.NormalizedKey;
                var itemPart = itemKey?.Split('.');
                var itemName = itemPart?.Length > 0 ? itemPart[0] : null;
                if (itemName == null) continue;

                var startsWithOpenBracket = itemName.StartsWith(normK + "[");
                if (startsWithOpenBracket == false) continue;

                var endsWithCloseBracket = itemName.EndsWith("]");
                if (endsWithCloseBracket == false) continue;

                var openBracketIndex = itemName.IndexOf('[');
                if (openBracketIndex < 0) continue;

                var k = itemName.Substring(openBracketIndex + 1, itemName.Length - openBracketIndex - 2);
                if (items.ContainsKey(k) == false) {
                    items.Add(k, "");
                }
                items[k] +=
                    string.Join(".", new[] { "_" }.Concat(itemPart.Skip(1))) +
                    "=" +
                    item.OriginalValue +
                    Environment.NewLine;
            }

            return items;
        }

        public static IEnumerable<T> All<T>(Func<T> factory, string key, IEqualityComparer<string> comparer, IConfBlock conf) {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            if (null == conf) throw new ArgumentNullException(nameof(conf));

            var normK = Key.Normalize(key ?? typeof(T).Name);
            var count = conf.ItemCount();
            var items = new HashSet<string>(comparer);
            for (var i = 0; i < count; i++) {
                var item = conf.Item(i);
                var itemKey = item.NormalizedKey;
                var itemPart = itemKey?.Split('.');
                var itemName = itemPart?.Length > 0 ? itemPart[0] : null;
                if (itemName == null) continue;

                var isItem = itemName == normK || (itemName.StartsWith(normK + "[") && itemName.EndsWith("]"));
                if (isItem == false) continue;

                items.Add(itemName);
            }

            foreach (var item in items) {
                yield return conf.Configure(factory(), item);
            }
        }

        public static IEnumerable<KeyValuePair<string, T>> Keyed<T>(Func<string, T> factory, string key, IEqualityComparer<string> comparer, IConfBlock conf) {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            var dict = Pair(key, comparer, conf);
            foreach (var pair in dict) {
                var k = pair.Key;
                var cnf = new ConfContainer { Content = pair.Value };
                var obj = factory(k);
                yield return new KeyValuePair<string, T>(k, cnf.Configure(obj, "_"));
            }
        }
    }
}
