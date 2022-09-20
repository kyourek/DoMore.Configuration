namespace Domore.Conf {
    public class Conf : IConf {
        public static ConfContainer Container { get; } = new ConfContainer();

        public static IConfContentProvider ContentProvider {
            get => Container.ContentProvider;
            set => Container.ContentProvider = value;
        }

        public static T Configure<T>(T obj, string key = null) => Container.Configure(obj, key);

        T IConf.Configure<T>(T target, string key) {
            return Container.Configure(target, key);
        }
    }
}
