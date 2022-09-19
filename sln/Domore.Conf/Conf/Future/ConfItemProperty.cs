using System;
using System.Collections;
using System.Reflection;

namespace Domore.Conf.Future {
    internal class ConfItemProperty : ConfTargetProperty {
        public override object PropertyValue {
            get => PropertyInfo.GetValue(Target, Index);
            set => PropertyInfo.SetValue(Target, value, Index);
        }

        public bool IndexExists {
            get {
                try {
                    PropertyInfo.GetValue(Target, Index);
                }
                catch {
                    return false;
                }
                return true;
            }
        }

        public ConfItemProperty(object target, ConfKeyPart key, ConfPropertyCache cache) : base(target, key, cache) {
        }

        public static ConfItemProperty Create(object target, ConfKeyPart key, ConfPropertyCache cache) {
            if (target is IList list) {
                return new ListProperty(list, key, cache);
            }
            if (target is IDictionary dictionary) {
                return new DictionaryProperty(dictionary, key, cache);
            }
            return new ConfItemProperty(target, key, cache);
        }

        private class ListProperty : ConfItemProperty {
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

            public ListProperty(IList list, ConfKeyPart key, ConfPropertyCache cache) : base(list, key, cache) {
                List = list ?? throw new ArgumentNullException(nameof(list));
            }
        }

        private class DictionaryProperty : ConfItemProperty {
            public IDictionary Dictionary { get; }
            public new object Index => base.Index[0];

            public override object PropertyValue {
                get => Dictionary[Index];
                set => Dictionary[Index] = value;
            }

            public DictionaryProperty(IDictionary dictionary, ConfKeyPart key, ConfPropertyCache cache) : base(dictionary, key, cache) {
                Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }
        }
    }
}
