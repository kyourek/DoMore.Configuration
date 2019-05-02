using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace Domore.Conf {
    public class AppSettingsProvider : IConfContentsProvider {
        static IEnumerable<KeyValuePair<string, string>> EmptySettings {
            get { yield break; }
        }

        static IEnumerable<KeyValuePair<string, string>> GetSettings(NameValueCollection collection) {
            if (collection == null) return EmptySettings;
            if (collection.HasKeys() == false) return EmptySettings;
            return collection.AllKeys.Select(key => new KeyValuePair<string, string>(key, collection[key]));
        }

        static IEnumerable<KeyValuePair<string, string>> GetSettings(KeyValueConfigurationCollection collection) {
            if (collection == null) return EmptySettings;
            return collection.AllKeys.Select(key => new KeyValuePair<string, string>(key, collection[key].Value));
        }

        static IEnumerable<KeyValuePair<string, string>> GetSettings(AppSettingsSection section) {
            if (section == null) return EmptySettings;
            return GetSettings(section.Settings);
        } 

        static IEnumerable<KeyValuePair<string, string>> GetSettings(string exePath) {
            return string.IsNullOrWhiteSpace(exePath)
                ? GetSettings(ConfigurationManager.AppSettings)
                : GetSettings(ConfigurationManager.OpenExeConfiguration(exePath)?.AppSettings);
        }

        public IEnumerable<KeyValuePair<string, string>> GetConfContents(object content) {
            return GetSettings(content?.ToString());
        }
    }
}
