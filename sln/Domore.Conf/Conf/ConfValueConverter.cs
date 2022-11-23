using System;

namespace Domore.Conf {
    public class ConfValueConverter {
        public virtual object Convert(string value, ConfValueConverterState state) {
            if (null == state) throw new ArgumentNullException(nameof(state));
            var converter = state.TypeConverter;
            try {
                return converter.ConvertFromString(value);
            }
            catch {
                if (state.Property.PropertyType == typeof(Type)) {
                    return Type.GetType(value, throwOnError: true, ignoreCase: true);
                }
                var type = Type.GetType(value, throwOnError: false, ignoreCase: true);
                if (type != null) {
                    return Activator.CreateInstance(type);
                }
                throw;
            }
        }
    }
}
