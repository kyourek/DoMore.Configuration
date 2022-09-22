using System.Collections.Generic;

namespace Domore.Conf {
    public class Conf : IConf {
        private static readonly Dictionary<string, ConfContainer> ContainerCache = new Dictionary<string, ConfContainer>();

        private static ConfContainer Container { get; } = new ConfContainer();

        public static object Source {
            get => Container.Source;
            set => Container.Source = value;
        }

        public static IConfContentProvider ContentProvider {
            get => Container.ContentProvider;
            set => Container.ContentProvider = value;
        }

        public static T Configure<T>(T obj, string key = null) => Container.Configure(obj, key);

        public static T Configure<T>(object source, string key = null) where T : new() {
            var obj = new T();
            var container = default(ConfContainer);

            if (source is string s) {
                var cache = ContainerCache;
                if (cache.TryGetValue(s, out container) == false) {
                    cache[s] = container = new ConfContainer { Source = s };
                }
            }

            container = container ?? new ConfContainer { Source = source };
            return container.Configure(obj, key);
        }

        public static IConfContainer Contain(object source) {
            return new ConfContainer { Source = source };
        }

        T IConf.Configure<T>(T target, string key) {
            return Configure(target, key);
        }
    }
}
