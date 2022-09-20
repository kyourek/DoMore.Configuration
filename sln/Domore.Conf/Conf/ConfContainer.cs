using System;
using System.Collections.Generic;
using System.Linq;

namespace Domore.Conf {
    public class ConfContainer : IConf {
        private ConfPopulator Populator =>
            _Populator ?? (
            _Populator = new ConfPopulator());
        private ConfPopulator _Populator;

        private IConfContent Content =>
            _Content ?? (
            _Content = ContentProvider.GetConfContent(Contents));
        private IConfContent _Content;

        public IConfContentProvider ContentProvider {
            get => _ContentProvider ?? (_ContentProvider = new ConfContentProvider());
            set {
                _ContentProvider = value;
                _Content = null;
            }
        }
        private IConfContentProvider _ContentProvider;

        public object Contents {
            get => _Contents;
            set {
                _Contents = value;
                _Content = null;
            }
        }
        private object _Contents;

        public T Configure<T>(T target, string key = null) {
            if (null == target) throw new ArgumentNullException(nameof(target));
            var k = key ?? typeof(T).Name;
            var p = k == "" ? Content.Pairs : Content.Pairs.Where(pair => pair.Key.StartsWith(k)).Select(pair => new ConfPair(pair.Key.Skip(), pair.Value));
            Populator.Populate(target, this, p);
            return target;
        }

        public IEnumerable<T> Configure<T>(Func<T> factory, string key = null, IEqualityComparer<string> comparer = null) {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            var k = key ?? typeof(T).Name;
            var groups = Content.Pairs
                .Where(pair => pair.Key.StartsWith(k))
                .Where(pair => pair.Key.Parts.Count > 0)
                .Where(pair => pair.Key.Parts[0].Indices.Count <= 1)
                .GroupBy(pair => pair.Key.Parts[0].Indices.Count == 1
                    ? pair.Key.Parts[0].Indices[0].Content
                    : null, comparer);
            foreach (var group in groups) {
                var target = factory();
                var pairs = group.Select(pair => new ConfPair(pair.Key.Skip(), pair.Value));
                Populator.Populate(target, this, pairs);
                yield return target;
            }
        }

        public IEnumerable<KeyValuePair<string, T>> Configure<T>(Func<string, T> factory, string key = null, IEqualityComparer<string> comparer = null) {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            var k = key ?? typeof(T).Name;
            var groups = Content.Pairs
                .Where(pair => pair.Key.StartsWith(k))
                .Where(pair => pair.Key.Parts.Count > 0)
                .Where(pair => pair.Key.Parts[0].Indices.Count <= 1)
                .GroupBy(pair => pair.Key.Parts[0].Indices.Count == 1
                    ? pair.Key.Parts[0].Indices[0].Content
                    : null, comparer);
            foreach (var group in groups) {
                var target = factory(group.Key);
                var pairs = group.Select(pair => new ConfPair(pair.Key.Skip(), pair.Value));
                Populator.Populate(target, this, pairs);
                yield return new KeyValuePair<string, T>(group.Key, target);
            }
        }
    }
}
