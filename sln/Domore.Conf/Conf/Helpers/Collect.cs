using System;
using System.Collections.Generic;

namespace Domore.Conf.Helpers {
    internal static class Collect {
        public static IEnumerable<T> All<T>(Func<T> factory, string key, IConfBlock conf) {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            if (null == conf) throw new ArgumentNullException(nameof(conf));

            var normK = Key.Normalize(key ?? typeof(T).Name);
            var count = conf.ItemCount();
            var items = new HashSet<string>();
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
    }
}
