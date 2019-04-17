using System;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    using Contents;

    [Guid("7160A870-3B2F-493A-8C48-E890BB75995A")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AppSettingsContainer : ConfigurationContainer {
        public AppSettingsContainer() {
            ContentsFactory = new AppSettingsFactory();
        }
    }
}
