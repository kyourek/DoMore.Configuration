using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("C6E7DC5A-1903-45C1-A2E2-A28EE2F36079")]
    [ComVisible(true)]
#if NETCOREAPP
    [ClassInterface(ClassInterfaceType.None)]
#else
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
#endif
    public class ConfigurationTypeConverter : TypeConverter {
        public IConfigurationBlock ConfigurationBlock { get; set; }
    }
}
