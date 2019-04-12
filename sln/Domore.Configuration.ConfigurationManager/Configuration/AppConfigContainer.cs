using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    using Helpers;

    [Guid("7160A870-3B2F-493A-8C48-E890BB75995A")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AppConfigContainer : ConfigurationContainer {
        private static readonly App App = new App();

        protected override string GetDefaultContent() {
            var value = base.GetDefaultContent();
            return string.IsNullOrWhiteSpace(value)
                ? string.Join(Environment.NewLine, App
                    .Settings
                    .Select(pair => string.Join(" = ", pair.Key, pair.Value)))
                : value;
        }

        public string ExePath {
            get { return App.Path; }
            set { App.Path = value; }
        }
    }
}
