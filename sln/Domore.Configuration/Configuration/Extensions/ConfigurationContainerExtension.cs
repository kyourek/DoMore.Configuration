using System;
namespace Domore.Configuration.Extensions {
    public static class ConfigurationContainerExtension {
        public static T Configure<T>(this IConfigurationContainer configurationContainer, T obj, string key = null) {
            if (null == configurationContainer) throw new ArgumentNullException(nameof(configurationContainer));
            return configurationContainer.Block.Configure(obj, key);
        }
    }
}
