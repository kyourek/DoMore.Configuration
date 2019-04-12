using System;
using System.Linq;
using System.Reflection;

namespace Domore.Configuration.Helpers {
    internal class Property {
        private static void SetOne(object obj, IConfigurationBlock block, string key, int dotCount) {
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

        public static object SetAll(object obj, IConfigurationBlock block, string key) {
            var normalizedKey = Key.Normalize(key);

            var dotCount = key.Count(c => c == '.');
            var itemCount = block.ItemCount;
            for (var i = 0; i < itemCount; i++) {
                var item = block.Item(i);
                if (item.NormalizedKey.StartsWith(normalizedKey + ".")) {
                    SetOne(obj, block, item.NormalizedKey, dotCount);
                }
            }

            return obj;
        }

        private class ObjectProperty {
            public object Object { get { return _Object; } }
            private readonly object _Object;

            public string Key { get { return _Key; } }
            private readonly string _Key;

            public Converter Converter {
                get { return _Converter ?? (_Converter = new Converter()); }
                set { _Converter = value; }
            }
            private Converter _Converter;

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
                            .Select((v, i) => Converter.Convert(parameters[i].ParameterType, v))
                            .ToArray();
                    }
                    return _Index;
                }
            }
            private object[] _Index;

            public Type ObjectType {
                get { return _ObjectType ?? (_ObjectType = Object.GetType()); }
            }
            private Type _ObjectType;

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
            private PropertyInfo _PropertyInfo;

            public Type PropertyType {
                get {
                    var pi = PropertyInfo;
                    if (pi == null) throw new InvalidOperationException("Property info cannot be null.");
                    return pi.PropertyType;
                }
            }

            public bool Exists {
                get { return PropertyInfo != null; }
            }

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
            private ItemProperty _Item;

            public virtual object PropertyValue {
                get { return PropertyInfo.GetValue(Object, null); }
                set { PropertyInfo.SetValue(Object, value, null); }
            }

            public ObjectProperty(object @object, string key) {
                if (null == @object) throw new ArgumentNullException("object");
                _Key = key;
                _Object = @object;
            }

            public void SetValue(IConfigurationBlock block, string key) {
                PropertyValue = Converter.Convert(PropertyType, block, key);
            }
        }

        private class ItemProperty : ObjectProperty {
            public ItemProperty(object @object, string key) : base(@object, key) {
            }

            public override object PropertyValue {
                get { return PropertyInfo.GetValue(Object, Index); }
                set { PropertyInfo.SetValue(Object, value, Index);; }
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
