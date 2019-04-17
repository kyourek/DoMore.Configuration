using System.Collections.Generic;

namespace Domore.Configuration.Contents {
    using Helpers;

    class AppSettingsFactory : IConfigurationContentsFactory {
        App _App;
        public App App {
            get => _App ?? (_App = new App());
            set => _App = value;
        }

        public IEnumerable<KeyValuePair<string, string>> CreateConfigurationContents(object content) {
            return App.GetSettings(content?.ToString());
        }
    }
}
