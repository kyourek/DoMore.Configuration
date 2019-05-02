using System.Runtime.InteropServices;

namespace Domore.Conf {
    [Guid("DB5DC8B5-6ED2-4A75-A88E-F9F9A84CF3F6")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Conf : IConf {
        public static IConfContainer Container { get; } = new ConfContainer();

        public static IConfContentsProvider ContentsProvider {
            get => Container.ContentsProvider;
            set => Container.ContentsProvider = value;
        }

        public static T Configure<T>(T obj, string key = null) => Container.Configure(obj, key);

        IConfContainer IConf.Container => Container;
    }
}
