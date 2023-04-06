using System;
using System.ComponentModel;
using System.Reflection;

namespace Domore.Conf {
    public sealed class ConfValueConverterState {
        public object Target { get; }
        public IConf Conf { get; }
        public PropertyInfo Property { get; }

        public TypeConverter TypeConverter =>
            _TypeConverter ?? (
            _TypeConverter = TypeDescriptor.GetConverter(Property.PropertyType));
        private TypeConverter _TypeConverter;

        public ConfValueConverterState(object target, PropertyInfo property, IConf conf) {
            Conf = conf;
            Target = target;
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }
    }
}
