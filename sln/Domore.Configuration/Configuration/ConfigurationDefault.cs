using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("DB5DC8B5-6ED2-4A75-A88E-F9F9A84CF3F6")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfigurationDefault : IConfigurationDefault {
        static readonly IConfigurationContainer _Container = new ConfigurationContainer();
        public IConfigurationContainer Container { get => _Container; }

        public static IConfigurationContentsFactory ContentsFactory {
            get => _Container.ContentsFactory;
            set => _Container.ContentsFactory = value;
        }

        public static T Configure<T>(T obj, string key = null) => _Container.Configure(obj, key);
    }
}
