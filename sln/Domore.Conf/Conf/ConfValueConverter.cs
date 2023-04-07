using System;
using System.Collections;

namespace Domore.Conf {
    using Converters;

    public class ConfValueConverter {
        private static readonly ConfValueConverter DefaultListItemsConverter = new ConfListItemsAttribute().ConverterInstance;

        internal static object Default(string value, ConfValueConverterState state) {
            if (null == state) throw new ArgumentNullException(nameof(state));
            var converter = state.TypeConverter;
            try {
                return converter.ConvertFromString(value);
            }
            catch {
                if (state.Property.PropertyType == typeof(Type)) {
                    return Type.GetType(value, throwOnError: true, ignoreCase: true);
                }
                if (typeof(IList).IsAssignableFrom(state.Property.PropertyType)) {
                    return DefaultListItemsConverter.Convert(value, state);
                }
                var type = Type.GetType(value, throwOnError: false, ignoreCase: true);
                if (type != null) {
                    return Activator.CreateInstance(type);
                }
                throw;
            }
        }

        public virtual object Convert(string value, ConfValueConverterState state) {
            try {
                return Default(value, state);
            }
            catch (Exception ex) {
                throw new ConfValueConverterException(this, value, state, ex);
            }
        }

        internal abstract class Internal : ConfValueConverter {
            protected abstract object Convert(bool @internal, string value, ConfValueConverterState state);

            public sealed override object Convert(string value, ConfValueConverterState state) {
                try {
                    return Convert(@internal: true, value, state);
                }
                catch (Exception ex) {
                    throw new ConfValueConverterException(this, value, state, ex);
                }
            }
        }
    }
}
