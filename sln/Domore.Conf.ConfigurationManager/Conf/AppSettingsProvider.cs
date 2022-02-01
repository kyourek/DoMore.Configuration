using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace Domore.Conf {
    public class AppSettingsProvider : IConfContentsProvider {
        private static IEnumerable<KeyValuePair<string, string>> EmptySettings {
            get { yield break; }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetSettings(NameValueCollection collection) {
            if (collection == null) return EmptySettings;
            if (collection.HasKeys() == false) return EmptySettings;
            return collection.AllKeys.Select(key => new KeyValuePair<string, string>(key, collection[key]));
        }

        private static IEnumerable<KeyValuePair<string, string>> GetSettings(KeyValueConfigurationCollection collection) {
            if (collection == null) return EmptySettings;
            return collection.AllKeys.Select(key => new KeyValuePair<string, string>(key, collection[key].Value));
        }

        private static IEnumerable<KeyValuePair<string, string>> GetSettings(AppSettingsSection section) {
            if (section == null) return EmptySettings;
            return GetSettings(section.Settings);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetSettings(string exePath) {
            return string.IsNullOrWhiteSpace(exePath)
                ? GetSettings(ConfigurationManager.AppSettings)
                : GetSettings(ConfigurationManager.OpenExeConfiguration(exePath)?.AppSettings);
        }

        public IEnumerable<KeyValuePair<string, string>> GetConfContents(object content) {
            return GetSettings(content?.ToString());
        }

        public string GetConfContent(IEnumerable<KeyValuePair<string, string>> contents) {
            contents = contents ?? new KeyValuePair<string, string>[] { };
            var appSettings = "<appSettings>" + Environment.NewLine;
            foreach (var item in contents) {
                appSettings += $"  <add key=\"{item.Key}\" value=\"{item.Value}\"/>";
                appSettings += Environment.NewLine;
            }
            appSettings += "</appSettings>";
            return appSettings;
        }
    }
}
