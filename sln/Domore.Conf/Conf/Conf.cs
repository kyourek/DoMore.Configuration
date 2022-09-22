using System;
using System.Collections.Generic;

namespace Domore.Conf {
    public class Conf : IConf {
        private static readonly Dictionary<string, ConfContainer> ContainerCache = new Dictionary<string, ConfContainer>();
        private static readonly ConfContainer Container = new ConfContainer();

        public static object Source {
            get => Container.Source;
            set => Container.Source = value;
        }

        public static IConfContentProvider ContentProvider {
            get => Container.ContentProvider;
            set => Container.ContentProvider = value;
        }

        public static T Configure<T>(T target, string key = null) {
            return Container.Configure(target, key);
        }

        public static IEnumerable<T> Configure<T>(Func<T> factory, string key = null, IEqualityComparer<string> comparer = null) {
            return Container.Configure(factory, key, comparer);
        }

        public static IEnumerable<KeyValuePair<string, T>> Configure<T>(Func<string, T> factory, string key = null, IEqualityComparer<string> comparer = null) {
            return Container.Configure(factory, key, comparer);
        }

        public static IConfContainer Contain(object source) {
            return new ConfContainer { Source = source };
        }

        public static IConfContainer Contain(string source) {
            var cache = ContainerCache;
            if (cache.TryGetValue(source, out var container) == false) {
                cache[source] = container = new ConfContainer { Source = source };
            }
            return container;
        }

        T IConf.Configure<T>(T target, string key) {
            return Configure(target, key);
        }

        IEnumerable<T> IConf.Configure<T>(Func<T> factory, string key, IEqualityComparer<string> comparer) {
            return Configure(factory, key, comparer);
        }

        IEnumerable<KeyValuePair<string, T>> IConf.Configure<T>(Func<string, T> factory, string key, IEqualityComparer<string> comparer) {
            return Configure(factory, key, comparer);
        }
    }
}
