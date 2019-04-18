using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("DB5DC8B5-6ED2-4A75-A88E-F9F9A84CF3F6")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfigurationDefault : IConfigurationDefault {
        public static IConfigurationContainer Container { get; } = new ConfigurationContainer();

        public static IConfigurationContentsFactory ContentsFactory {
            get => Container.ContentsFactory;
            set => Container.ContentsFactory = value;
        }

        public static T Configure<T>(T obj, string key = null) => Container.Configure(obj, key);

        IConfigurationContainer IConfigurationDefault.Container => Container;
    }
}
