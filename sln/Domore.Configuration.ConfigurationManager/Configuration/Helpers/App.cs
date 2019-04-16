using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace Domore.Configuration.Helpers {
    using Configuration = System.Configuration.Configuration;

    class App {
        static IEnumerable<KeyValuePair<string, string>> EmptySettings {
            get {
                yield break;
            }
        }

        static IEnumerable<KeyValuePair<string, string>> GetConfigurationSettings(NameValueCollection collection) {
            if (collection == null) return EmptySettings;
            if (collection.HasKeys() == false) return EmptySettings;
            return collection.AllKeys.Select(key => new KeyValuePair<string, string>(key, collection[key]));
        }

        static IEnumerable<KeyValuePair<string, string>> GetConfigurationSettings(KeyValueConfigurationCollection collection) {
            if (collection == null) return EmptySettings;
            return collection.AllKeys.Select(key => new KeyValuePair<string, string>(key, collection[key].Value));
        }

        static IEnumerable<KeyValuePair<string, string>> GetConfigurationSettings(AppSettingsSection section) {
            if (section == null) return EmptySettings;
            return GetConfigurationSettings(section.Settings);
        }

        Configuration GetConfiguration() {
                var path = Path;
                return string.IsNullOrWhiteSpace(path)
                    ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel)
                    : ConfigurationManager.OpenExeConfiguration(path);
        }

        void Reset() {
            Settings = null;
            Configuration = null;
            ConfigurationError = null;
        }

        string _Path;
        public string Path {
            get { return _Path; }
            set {
                if (_Path != value) {
                    _Path = value;
                    Reset();
                }
            }
        }

        public Configuration Configuration { get; private set; }
        public Exception ConfigurationError { get; private set; }

        ConfigurationUserLevel _ConfigurationUserLevel = ConfigurationUserLevel.None;
        public ConfigurationUserLevel ConfigurationUserLevel {
            get { return _ConfigurationUserLevel; }
            set {
                if (_ConfigurationUserLevel != value) {
                    _ConfigurationUserLevel = value;
                    Reset();
                }
            }
        }

        IEnumerable<KeyValuePair<string, string>> _Settings;
        public IEnumerable<KeyValuePair<string, string>> Settings {
            get {
                if (_Settings == null) {
                    var configuration = default(Configuration);
                    try {
                        configuration = Configuration = GetConfiguration();
                    }
                    catch (Exception ex) {
                        ConfigurationError = ex;
                    }

                    _Settings = configuration == null
                        ? GetConfigurationSettings(ConfigurationManager.AppSettings)
                        : GetConfigurationSettings(configuration.AppSettings);
                }
                return _Settings;
            }
            private set {
                _Settings = value;
            }
        }
    }
}
