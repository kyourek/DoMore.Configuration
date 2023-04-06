using System;
using System.Linq;

namespace Domore.Conf.Converters {
    public sealed class ConfEnumFlagsAttribute : ConfConverterAttribute {
        internal sealed override ConfValueConverter ConverterInstance =>
            _ConverterInstance ?? (
            _ConverterInstance = new ValueConverter { Separators = Separators });
        private ConfValueConverter _ConverterInstance;

        public string Separators {
            get => _Separators;
            set {
                if (_Separators != value) {
                    _Separators = value;
                    _ConverterInstance = null;
                }
            }
        }
        private string _Separators;

        private sealed class ValueConverter : ConfValueConverter.Internal {
            private static readonly string DefaultSeparators = "+|&,/\\";

            protected sealed override object Convert(bool @interna, string value, ConfValueConverterState state) {
                if (null == value) throw new ArgumentNullException(nameof(value));
                if (null == state) throw new ArgumentNullException(nameof(state));
                var type = state.Property.PropertyType;
                var names = type.GetEnumNames();
                var separators = Separators;
                if (separators == null || separators.Length == 0) {
                    separators = DefaultSeparators;
                }
                foreach (var c in separators) {
                    if (value.Contains(c)) {
                        var parsableString = string.Join(",", value.Split(c).Select(s => s?.Trim() ?? "").Where(s => s != ""));
                        var parseResult = Enum.Parse(type, parsableString, ignoreCase: true);
                        return parseResult;
                    }
                }
                return Default(value, state);
            }

            public string Separators { get; set; }
        }
    }
}
