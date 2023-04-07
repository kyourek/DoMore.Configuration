using System;
using System.Collections.Generic;
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
            private static readonly Dictionary<Type, Dictionary<string, HashSet<string>>> AliasCache = new Dictionary<Type, Dictionary<string, HashSet<string>>>();

            private static Dictionary<string, HashSet<string>> Alias(Type type) {
                if (null == type) throw new ArgumentNullException(nameof(type));
                var dict = new Dictionary<string, HashSet<string>>();
                var names = type.GetEnumNames();
                foreach (var name in names) {
                    var member = type.GetMember(name).FirstOrDefault(m => m.DeclaringType == type);
                    if (member == null) {
                        continue;
                    }
                    var set = new HashSet<string>(new[] { name }, StringComparer.OrdinalIgnoreCase);
                    var conf = member.GetCustomAttributes(typeof(ConfAttribute), inherit: true).OfType<ConfAttribute>().FirstOrDefault();
                    if (conf != null) {
                        foreach (var alias in conf.Names) {
                            set.Add(alias);
                        }
                    }
                    dict[name] = set;
                }
                return dict;
            }

            protected sealed override object Convert(bool @interna, string value, ConfValueConverterState state) {
                if (null == value) throw new ArgumentNullException(nameof(value));
                if (null == state) throw new ArgumentNullException(nameof(state));
                var type = state.Property.PropertyType;
                var alias = AliasCache.ContainsKey(type) ? AliasCache[type] : (AliasCache[type] = Alias(type));
                var separators = Separators;
                if (separators == null || separators.Length == 0) {
                    separators = DefaultSeparators;
                }
                foreach (var c in separators) {
                    if (value.Contains(c)) {
                        var items = value
                            .Split(c)
                            .Select(s => s?.Trim() ?? "")
                            .Where(s => s != "")
                            .Select(s => alias.FirstOrDefault(pair => pair.Value.Contains(s)).Key ?? s);
                        var parsableString = string.Join(",", items);
                        var parseResult = Enum.Parse(type, parsableString, ignoreCase: true);
                        return parseResult;
                    }
                }
                value = alias.FirstOrDefault(pair => pair.Value.Contains(value)).Key ?? value;
                return Enum.Parse(type, value, ignoreCase: true);
            }

            public string Separators { get; set; }
        }
    }
}
