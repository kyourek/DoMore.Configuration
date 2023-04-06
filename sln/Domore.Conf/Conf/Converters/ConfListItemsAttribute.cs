using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace Domore.Conf.Converters {
    internal sealed class ConfListItemsAttribute : ConfAttribute {
        internal sealed override ConfValueConverter ConverterInstance =>
            _ConverterInstance ?? (
            _ConverterInstance = new ValueConverter {
                ItemConverter = ItemConverter == null
                    ? null
                    : Activator.CreateInstance(ItemConverter)
            });
        private ConfValueConverter _ConverterInstance;

        public Type ItemConverter { get; }

        public ConfListItemsAttribute(params string[] names) : base(names) {
        }

        public ConfListItemsAttribute(Type itemConverter, params string[] names) : this(names) {
            ItemConverter = itemConverter;
        }

        private sealed class ValueConverter : ConfValueConverter.Internal {
            protected sealed override object Convert(bool @internal, string value, ConfValueConverterState state) {
                if (null == value) throw new ArgumentNullException(nameof(value));
                if (null == state) throw new ArgumentNullException(nameof(state));
                var obj = state.Property.GetValue(state.Target, null);
                if (obj == null) {
                    obj = Activator.CreateInstance(state.Property.PropertyType);
                }
                var list = (IList)obj;
                var itemConverter = ItemConverter;
                if (itemConverter == null) {
                    var listType = list.GetType();
                    var itemType = listType.GetGenericArguments().FirstOrDefault();
                    itemConverter = itemType == null
                        ? null
                        : TypeDescriptor.GetConverter(itemType);
                }
                var typeConverter = itemConverter as TypeConverter;
                var valueConverter = itemConverter as ConfValueConverter;
                var itemStrings = value.Split(',').Select(s => s?.Trim() ?? "").Where(s => s != "");
                foreach (var itemString in itemStrings) {
                    if (typeConverter != null) {
                        list.Add(typeConverter.ConvertFromString(itemString));
                        continue;
                    }
                    if (valueConverter != null) {
                        list.Add(valueConverter.Convert(itemString, state));
                        continue;
                    }
                    list.Add(itemString);
                }
                return list;
            }

            public object ItemConverter { get; set; }
        }
    }
}
