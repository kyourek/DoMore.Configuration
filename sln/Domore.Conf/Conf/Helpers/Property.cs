using System;
using System.Linq;
using System.Reflection;

namespace Domore.Conf.Helpers {
    class Property {
        static void SetOne(object obj, IConfBlock block, string key, int dotCount) {
            var parts = key.Split('.');
            for (var i = dotCount + 1; i < parts.Length; i++) {
                var propertyKey = parts[i];
                var objectProperty = new ObjectProperty(obj, propertyKey);
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

        public static object SetAll(object obj, IConfBlock block, string key) {
            var normalizedKey = Key.Normalize(key);

            var dotCount = key.Count(c => c == '.');
            var itemCount = block.ItemCount();
            for (var i = 0; i < itemCount; i++) {
                var item = block.Item(i);
                if (item.NormalizedKey.StartsWith(normalizedKey + ".")) {
                    SetOne(obj, block, item.NormalizedKey, dotCount);
                }
            }

            return obj;
        }

        class ObjectProperty {
            public object Object { get; }
            public string Key { get; }

            private string _PropertyName;
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
                            : new ItemProperty(PropertyValue, "Item" + IndexString);
                    }
                    return _Item;
                }
            }

            public virtual object PropertyValue {
                get => PropertyInfo.GetValue(Object, null);
                set => PropertyInfo.SetValue(Object, value, null);
            }

            public ObjectProperty(object @object, string key) {
                if (null == @object) throw new ArgumentNullException(nameof(@object));
                Key = key;
                Object = @object;
            }

            public void SetValue(IConfBlock block, string key) {
                PropertyValue = Converter.Convert(PropertyType, block, key);
            }
        }

        class ItemProperty : ObjectProperty {
            public ItemProperty(object @object, string key) : base(@object, key) {
            }

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
        }
    }
}
