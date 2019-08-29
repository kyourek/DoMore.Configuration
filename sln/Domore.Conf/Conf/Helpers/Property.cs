using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Domore.Conf.Helpers {
    static class Property {
        static void SetOne(object obj, bool keyed, IConfBlock block, string key, int dotCount, ConfConverter converter) {
            var parts = key.Split('.');
            var start = keyed ? dotCount + 1 : dotCount;
            for (var i = start; i < parts.Length; i++) {
                var propertyKey = parts[i];
                var objectProperty = new ObjectProperty(obj, propertyKey, converter);
                if (objectProperty.Exists) {
                    if (i == parts.Length - 1) {
                        var item = objectProperty.Item;
                        if (item != null && item.Exists) {
                            item.SetValue(block, key);
                        }
                        else {
                            objectProperty.SetValue(block, key);
                        }
                        break;
                    }
                    else {
                        if (objectProperty.PropertyValue == null) {
                            objectProperty.SetValue(block, string.Join(".", parts.Take(i + 1)));
                        }

                        var item = objectProperty.Item;
                        if (item != null && item.Exists && (item.IndexExists == false || item.PropertyValue == null)) {
                            item.SetValue(block, string.Join(".", parts.Take(i + 1)));
                        }

                        obj = item != null && item.Exists
                            ? item.PropertyValue
                            : objectProperty.PropertyValue;
                    }
                }
                else {
                    break;
                }
            }
        }

        public static object SetAll(object obj, IConfBlock block, string key, ConfConverter converter) {
            var normKey = Key.Normalize(key);
            var noKey = normKey == "";

            var dotCount = key.Count(c => c == '.');
            var itemCount = block.ItemCount();
            for (var i = 0; i < itemCount; i++) {
                var item = block.Item(i);
                if (item.NormalizedKey.StartsWith(normKey + ".") || noKey) {
                    SetOne(obj, !noKey, block, item.NormalizedKey, dotCount, converter);
                }
            }

            return obj;
        }

        class ObjectProperty {
            public object Object { get; }
            public string Key { get; }
            public ConfConverter Converter { get; }

            string _PropertyName;
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

            string _IndexString;
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

            object[] _Index;
            public object[] Index {
                get {
                    if (_Index == null) {
                        var str = IndexString;
                        if (str == "") return null;

                        var csv = str.Substring(1, str.Length - 2);
                        var array = csv.Split(',');
                        var parameters = PropertyInfo.GetIndexParameters();

                        _Index = array
                            .Select((v, i) => Converter.Convert(parameters[i].ParameterType, v))
                            .ToArray();
                    }
                    return _Index;
                }
            }

            Type _ObjectType;
            public Type ObjectType => _ObjectType ?? (_ObjectType = Object.GetType());

            PropertyInfo _PropertyInfo;
            public PropertyInfo PropertyInfo {
                get {
                    if (_PropertyInfo == null) {
                        _PropertyInfo = ObjectType
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .FirstOrDefault(property => string.Equals(property.Name, PropertyName, StringComparison.OrdinalIgnoreCase));
                    }
                    return _PropertyInfo;
                }
            }

            public Type PropertyType {
                get {
                    var pi = PropertyInfo;
                    if (pi == null) throw new InvalidOperationException("Property info cannot be null.");
                    return pi.PropertyType;
                }
            }

            public bool Exists => PropertyInfo != null;

            ItemProperty _Item;
            public ItemProperty Item {
                get {
                    if (_Item == null) {
                        _Item = IndexString == ""
                            ? null
                            : ItemProperty.Create(PropertyValue, $"Item{IndexString}", Converter);
                    }
                    return _Item;
                }
            }

            public virtual object PropertyValue {
                get => PropertyInfo.GetValue(Object, null);
                set => PropertyInfo.SetValue(Object, value, null);
            }

            public ObjectProperty(object @object, string key, ConfConverter converter) {
                Key = key;
                Object = @object ?? throw new ArgumentNullException(nameof(@object));
                Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            }

            public void SetValue(IConfBlock block, string key) {
                PropertyValue = Converter.Convert(PropertyType, block, key);
            }
        }

        class ItemProperty : ObjectProperty {
            public override object PropertyValue {
                get => PropertyInfo.GetValue(Object, Index);
                set => PropertyInfo.SetValue(Object, value, Index);
            }

            public bool IndexExists {
                get {
                    try {
                        PropertyInfo.GetValue(Object, Index);
                    }
                    catch {
                        return false;
                    }
                    return true;
                }
            }

            public ItemProperty(object @object, string key, ConfConverter converter) : base(@object, key, converter) {
            }

            public static ItemProperty Create(object @object, string key, ConfConverter converter) {
                if (@object is IList list) {
                    return new ListProperty(list, key, converter);
                }
                if (@object is IDictionary dictionary) {
                    return new DictionaryProperty(dictionary, key, converter);
                }
                return new ItemProperty(@object, key, converter);
            }

            class ListProperty : ItemProperty {
                public IList List { get; }
                public new int Index => (int)base.Index[0];

                public override object PropertyValue {
                    get => List[Index];
                    set {
                        var type = PropertyType;
                        var list = List;
                        var index = Index;
                        var @default = type.IsValueType
                            ? Activator.CreateInstance(type)
                            : null;

                        while (list.Count < index) {
                            list.Add(@default);
                        }
                        if (list.Count > index) {
                            list[index] = value;
                        }
                        else {
                            list.Add(value);
                        }
                    }
                }

                public ListProperty(IList list, string key, ConfConverter converter) : base(list, key, converter) {
                    List = list ?? throw new ArgumentNullException(nameof(list));
                }
            }

            class DictionaryProperty : ItemProperty {
                public IDictionary Dictionary { get; }
                public new object Index => base.Index[0];

                public override object PropertyValue {
                    get => Dictionary[Index];
                    set => Dictionary[Index] = value;
                }

                public DictionaryProperty(IDictionary dictionary, string key, ConfConverter converter) : base(dictionary, key, converter) {
                    Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
                }
            }
        }
    }
}
