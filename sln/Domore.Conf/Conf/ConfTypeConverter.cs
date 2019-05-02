using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Domore.Conf {
    [Guid("C6E7DC5A-1903-45C1-A2E2-A28EE2F36079")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfTypeConverter : TypeConverter {
        internal IConfBlock Conf { get; set; }

        public virtual object ConvertFrom(IConfBlock conf, ITypeDescriptorContext context, CultureInfo culture, object value) {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            return ConvertFrom(Conf, context, culture, value);
        }
    }
}
