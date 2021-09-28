using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Domore.Conf.Helpers {
    internal static class Collect {
        private static NameValueCollection Pairs(string collection, IEqualityComparer<string> comparer, IConfBlock conf) {
            if (null == conf) throw new ArgumentNullException(nameof(conf));

            var normC = Key.Normalize(collection);
            var count = conf.ItemCount();
            var items = new NameValueCollection(new EqualityWrapper(comparer));
            for (var i = 0; i < count; i++) {
                var confItem = conf.Item(i);
                var thisItem = new Item(confItem.NormalizedKey);
                if (thisItem.Collection != normC) continue;

                var indx = thisItem.Index;
                var rest = thisItem.Rest;
                var valu = confItem.OriginalValue;
                items[indx] += $"{rest} = {valu}" + Environment.NewLine;
            }

            return items;
        }

        public static IEnumerable<T> All<T>(Func<T> factory, string collection, IEqualityComparer<string> comparer, IConfBlock conf) {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            if (null == conf) throw new ArgumentNullException(nameof(conf));

            var normK = Key.Normalize(collection ?? typeof(T).Name);
            var count = conf.ItemCount();
            var items = new HashSet<string>(comparer);
            for (var i = 0; i < count; i++) {
                var confItem = conf.Item(i);
                var thisItem = new Item(confItem.NormalizedKey);
                if (thisItem.Collection != normK) continue;
                items.Add(thisItem.Name);
            }

            foreach (var item in items) {
                yield return conf.Configure(factory(), item);
            }
        }

        public static IEnumerable<KeyValuePair<string, T>> Indexed<T>(Func<string, T> factory, string collection, IEqualityComparer<string> comparer, IConfBlock conf) {
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            var coll = collection ?? typeof(T).Name;
            var dict = Pairs(coll, comparer, conf);
            var keys = dict.AllKeys;
            foreach (var key in keys) {
                var val = dict[key];
                var cnf = new ConfContainer { Content = val };
                var obj = factory(key);
                yield return new KeyValuePair<string, T>(key, cnf.Configure(obj, ""));
            }
        }

        private class Item {
            private string[] Parts =>
                _Parts ?? (
                _Parts = Key.Split('.'));
            private string[] _Parts;

            public string Name =>
                _Name ?? (
                _Name = Parts[0]);
            private string _Name;

            public string Rest =>
                _Rest ?? (
                _Rest = string.Join(".", Parts.Skip(1)));
            private string _Rest;

            public string Collection =>
                _Collection ?? (
                _Collection = Indexed
                    ? new string(Name.TakeWhile(c => c != '[').ToArray())
                    : Name);
            private string _Collection;

            public bool Indexed =>
                _Indexed ?? (
                _Indexed = Name.Contains("[") && Name.EndsWith("]")).Value;
            private bool? _Indexed;

            public string Index {
                get {
                    if (_Index == null) {
                        var name = Name;
                        var closed = name.EndsWith("]");
                        if (closed == false) return null;

                        var openBracketIndex = name.IndexOf('[');
                        if (openBracketIndex < 0) return null;

                        _Index = name.Substring(openBracketIndex + 1, name.Length - openBracketIndex - 2);
                    }
                    return _Index;
                }
            }
            private string _Index;

            public string Key { get; }

            public Item(string key) {
                Key = key ?? throw new ArgumentNullException(nameof(key));
            }
        }

        private class EqualityWrapper : IEqualityComparer {
            public IEqualityComparer<string> Agent { get; }

            public EqualityWrapper(IEqualityComparer<string> agent) {
                Agent = agent ?? EqualityComparer<string>.Default;
            }

            public new bool Equals(object x, object y) {
                if (x is string xs && y is string ys) {
                    return Agent.Equals(xs, ys);
                }
                return EqualityComparer<object>.Default.Equals(x, y);
            }

            public int GetHashCode(object obj) {
                if (obj is string s) {
                    return Agent.GetHashCode(s);
                }
                return EqualityComparer<object>.Default.GetHashCode(obj);
            }
        }
    }
}
