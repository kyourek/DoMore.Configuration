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
        private void Configuration_Changed(object sender, EventArgs e) {
            OnConfigurationChanged(e);
        }

        protected virtual void OnConfigurationChanged(EventArgs e) {
            var handler = ConfigurationChanged;
            if (handler != null) handler.Invoke(this, e);
        }

        public event EventHandler ConfigurationChanged;

        private IConfigurationContainer _Configuration;
        public IConfigurationContainer Configuration {
            get {
                if (_Configuration == null) {
                    _Configuration = new ConfigurationContainer();
                    _Configuration.Changed += Configuration_Changed;
                }
                return _Configuration;
            }
        }
    }
}
