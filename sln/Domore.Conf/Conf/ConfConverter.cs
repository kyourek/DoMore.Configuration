using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domore.Conf {
    using Helpers;

    class ConfConverter : IConfConverterTable {
        readonly TypeConverterTable TypeConverters = new TypeConverterTable();

        public TypeConverter this[Type type] {
            set { TypeConverters[type] = value; }
        }

        Log _Log;
        public Log Log {
            get => _Log ?? (_Log = new Log());
            set => _Log = value;
        }

        public object Convert(Type type, string value, TypeConverter typeConverter = null) {
            Log.Lines("Converting value", value, "to type", type);
            typeConverter = typeConverter ?? TypeConverters.GetTypeConverter(type);
            Log.Lines("using converter", typeConverter);
            try {
                return typeConverter.ConvertFrom(value);
            }
            catch {
                if (type == typeof(Type)) {
                    return Type.GetType(value, throwOnError: true, ignoreCase: true);
                }
                var valueType = Type.GetType(value, throwOnError: false, ignoreCase: true);
                if (valueType != null) {
                    return Activator.CreateInstance(valueType);
                }
                throw;
            }
        }

        public object Convert(Type type, IConfBlock block, string key) {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == block) throw new ArgumentNullException(nameof(block));

            Log.Lines("Converting key", key, "to type", type);

            var conv = TypeConverters.GetTypeConverter(type);
            if (conv is ConfTypeConverter conf) {
                conf.Conf = block;
            }

            Log.Lines("using converter", conv);

            if (block.ItemExists(key, out var item)) {
                return Convert(type, item.OriginalValue, conv);
            }

            try {
                return conv.ConvertFrom(key);
            }
            catch {
                var constructor = type.GetConstructor(new Type[] { });
                if (constructor != null) {
                    return constructor.Invoke(new object[] { });
                }
                throw;
            }
        }

        class TypeConverterTable {
            readonly IDictionary<Type, TypeConverter> TypeConverters = new Dictionary<Type, TypeConverter>();

            void SetTypeConverter(Type type, TypeConverter converter) {
                lock (TypeConverters) {
                    TypeConverters[type] = converter;
                }
            }

            public TypeConverter GetTypeConverter(Type type) {
                if (null == type) throw new ArgumentNullException(nameof(type));
                lock (TypeConverters) {
                    if (TypeConverters.TryGetValue(type, out var typeConverter) == false || typeConverter == null) {
                        typeConverter = TypeDescriptor.GetConverter(type);
                    }
                    return typeConverter;
                }
            }

            public TypeConverter this[Type key] {
                set => SetTypeConverter(key, value);
            }
        }
    }
}
