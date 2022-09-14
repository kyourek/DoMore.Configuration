using System;
using System.Linq;
using System.Reflection;

namespace Domore.Conf.Future {
    internal class ConfObjectProperty {
        public object Object { get; }
        public string Key { get; }
        public ConfPropertyCache Cache { get; }

        public string PropertyName {
            get {
                if (_PropertyName == null) {
                    _PropertyName = Key.Contains("[")
                        ? new string(Key.TakeWhile(c => c != '[').ToArray())
                        : Key;
                }
                return _PropertyName;
            }
        }
        private string _PropertyName;

        public string IndexString {
            get {
                if (_IndexString == null) {
                    _IndexString = Key.Contains("[")
                        ? new string(Key.SkipWhile(c => c != '[').ToArray())
                        : "";
                }
                return _IndexString;
            }
        }
        private string _IndexString;

        public object[] Index {
            get {
                if (_Index == null) {
                    var str = IndexString;
                    if (str == "") return null;

                    var csv = str.Substring(1, str.Length - 2);
                    var array = csv.Split(',');
                    var parameters = PropertyInfo.GetIndexParameters();

                    _Index = array
                        .Select((v, i) => Convert.ChangeType(v, parameters[i].ParameterType))
                        .ToArray();
                }
                return _Index;
            }
        }
        private object[] _Index;

        public Type ObjectType =>
            _ObjectType ?? (
            _ObjectType = Object.GetType());
        private Type _ObjectType;

        public PropertyInfo PropertyInfo =>
            _PropertyInfo ?? (
            _PropertyInfo = Cache.Get(ObjectType, PropertyName));
        private PropertyInfo _PropertyInfo;

        public Type PropertyType {
            get {
                var pi = PropertyInfo;
                if (pi == null) throw new InvalidOperationException("Property info cannot be null.");
                return pi.PropertyType;
            }
        }

        public ConfAttribute Attribute =>
            _Attribute ?? (
            _Attribute = PropertyInfo.GetCustomAttributes(typeof(ConfAttribute), inherit: true)?.FirstOrDefault() as ConfAttribute);
        private ConfAttribute _Attribute;

        public bool Exists =>
            _Exists ?? (
            _Exists = PropertyInfo != null).Value;
        private bool? _Exists;

        public bool Populate =>
            _Populate ?? (
            _Populate = Attribute?.IgnoreSet != true).Value;
        private bool? _Populate;

        public ConfItemProperty Item {
            get {
                if (_Item == null) {
                    _Item = IndexString == ""
                        ? null
                        : ConfItemProperty.Create(PropertyValue, $"Item{IndexString}", Cache);
                }
                return _Item;
            }
        }
        private ConfItemProperty _Item;

        public virtual object PropertyValue {
            get => PropertyInfo.GetValue(Object, null);
            set => PropertyInfo.SetValue(Object, value, null);
        }

        public ConfObjectProperty(object @object, string key, ConfPropertyCache cache) {
            Key = key;
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Object = @object ?? throw new ArgumentNullException(nameof(@object));
        }
    }
}
