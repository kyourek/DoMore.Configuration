using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    using Contents;
    using Helpers;

    [Guid("7506691D-E25D-4D44-BBEF-5CDE862E9148")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfigurationBlockFactory : IConfigurationBlockFactory {
        public IConfigurationBlock CreateConfigurationBlock(object content, IConfigurationContentsFactory contentsFactory = null) {
            return new ConfigurationBlock(content, contentsFactory ?? new ConfContentsFactory());
        }

        class ConfigurationBlock : IConfigurationBlock {
            ConfigurationBlockItem.Collection _Items;
            ConfigurationBlockItem.Collection Items => _Items ?? (_Items =
                ConfigurationBlockItem.Collection.Create(ContentsFactory.CreateConfigurationContents(Content)));

            public object Content { get; }
            public IConfigurationContentsFactory ContentsFactory { get; }

            public int ItemCount => Items.Count;

            public ConfigurationBlock(object content, IConfigurationContentsFactory contentsFactory) {
                Content = content;
                ContentsFactory = contentsFactory;
            }

            public bool ItemExists(object key) {
                var error = default(Exception);
                try {
                    Item(key);
                }
                catch (Exception ex) {
                    error = ex;
                }
                return error == null;
            }

            public IConfigurationBlockItem Item(object key) {
                if (null == key) throw new ArgumentNullException(nameof(key));

                if (key is int index) {
                    return Items[index];
                }

                var stringKey = key.ToString();
                var normalizedKey = Key.Normalize(stringKey);
                return Items[normalizedKey];
            }

            public object Configure(object obj, string key) {
                if (null == obj) throw new ArgumentNullException(nameof(obj));

                key = (key ?? "").Trim();
                key = key == "" ? obj.GetType().Name : key;

                return Property.SetAll(obj, this, key);
            }

            public T Configure<T>(T obj, string key) {
                return (T)Configure((object)obj, key);
            }

            public override string ToString() {
                return string.Join(Environment.NewLine, Items);
            }
        }

        class ConfigurationBlockItem : IConfigurationBlockItem {
            public string OriginalKey { get; }
            public string NormalizedKey { get; }
            public string OriginalValue { get; }

            public ConfigurationBlockItem(string originalKey, string normalizedKey, string originalValue) {
                OriginalKey = originalKey;
                NormalizedKey = normalizedKey;
                OriginalValue = originalValue;
            }

            public object ConvertValue(Type type) {
                return Converter.Convert(type, OriginalValue);
            }

            public T ConvertValue<T>() {
                return (T)ConvertValue(typeof(T));
            }

            public override string ToString() {
                return $"{OriginalKey} = {OriginalValue}";
            }

            public class Collection : KeyedCollection<string, IConfigurationBlockItem> {
                void Add(IEnumerable<IConfigurationBlockItem> items) {
                    if (null == items) throw new ArgumentNullException(nameof(items));
                    foreach (var item in items) {
                        var key = GetKeyForItem(item);
                        if (Contains(key)) {
                            var old = this[key];
                            var index = IndexOf(old);
                            SetItem(index, item);
                        }
                        else {
                            Add(item);
                        }
                    }
                }

                protected override string GetKeyForItem(IConfigurationBlockItem item) {
                    if (null == item) throw new ArgumentNullException(nameof(item));
                    return item.NormalizedKey;
                }

                public Collection(IEnumerable<IConfigurationBlockItem> items) {
                    Add(items);
                }

                public static Collection Create(IEnumerable<KeyValuePair<string, string>> items) {
                    if (null == items) throw new ArgumentNullException(nameof(items));
                    return new Collection(items.Select(item => new ConfigurationBlockItem(item.Key, Key.Normalize(item.Key), item.Value)));
                }
            }
        }
    }
}
