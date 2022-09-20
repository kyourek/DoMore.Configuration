using System;
using System.Linq;
using System.Reflection;

namespace Domore.Conf.Future {
    internal class ConfTargetProperty {
        public object Target { get; }
        public IConfKeyPart Key { get; }
        public ConfPropertyCache Cache { get; }

        private ConfProperty Property =>
            _Property ?? (
            _Property = Cache.Get(TargetType, Key.Content));
        private ConfProperty _Property;

        public string IndexString =>
            _IndexString ?? (
            _IndexString = string.Join("", Key.Indices.Select(i => $"[{string.Join(",", i.Parts.Select(p => p.Content))}]")));
        private string _IndexString;

        public object[] Index {
            get {
                if (_Index == null) {
                    var indices = Key.Indices;
                    if (indices.Count == 0) {
                        return null;
                    }
                    var parameters = PropertyInfo.GetIndexParameters();

                    _Index = indices[0].Parts // TODO: Allow multiple indices.
                        .Select((v, i) => Convert.ChangeType(v.Content, parameters[i].ParameterType))
                        .ToArray();
                }
                return _Index;
            }
        }
        private object[] _Index;

        public Type TargetType =>
            _TargetType ?? (
            _TargetType = Target.GetType());
        private Type _TargetType;

        public PropertyInfo PropertyInfo =>
            _PropertyInfo ?? (
            _PropertyInfo = Property.PropertyInfo);
        private PropertyInfo _PropertyInfo;

        public Type PropertyType =>
            _PropertyType ?? (
            _PropertyType = PropertyInfo.PropertyType);
        private Type _PropertyType;

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
                    _Item = Key.Indices.Count == 0
                        ? null
                        : ConfItemProperty.Create(PropertyValue, new ItemKey(Key.Indices[0]), Cache);
                }
                return _Item;
            }
        }
        private ConfItemProperty _Item;

        public virtual object PropertyValue {
            get => PropertyInfo.GetValue(Target, null);
            set => PropertyInfo.SetValue(Target, value, null);
        }

        public ConfTargetProperty(object target, IConfKeyPart key, ConfPropertyCache cache) {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        private class ItemKey : IConfKeyPart {
            public string Content { get; }
            public IConfKeyIndex Index { get; }
            public IConfCollection<IConfKeyIndex> Indices { get; }

            public ItemKey(IConfKeyIndex index) {
                Index = index;
                Content = "Item";
                Indices = new ConfCollection<IConfKeyIndex>(Index);
            }
        }
    }
}
