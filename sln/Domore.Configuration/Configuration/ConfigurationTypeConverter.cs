using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("C6E7DC5A-1903-45C1-A2E2-A28EE2F36079")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfigurationTypeConverter : TypeConverter {
        internal IConfigurationBlock Configuration { get; set; }

        public virtual object ConvertFrom(IConfigurationBlock configuration, ITypeDescriptorContext context, CultureInfo culture, object value) {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            return ConvertFrom(Configuration, context, culture, value);
        }
    }
}
