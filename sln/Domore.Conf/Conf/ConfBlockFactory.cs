using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf {
    using Helpers;
    using Providers;

    class ConfBlockFactory {
        public IConfBlock CreateConfBlock(object content, IConfContentsProvider contentsProvider = null) {
            return new ConfBlock(content, contentsProvider ?? new ConfContentsProvider());
        }

        class ConfBlock : IConfBlock {
            ConfBlockItem.Collection _Items;
            ConfBlockItem.Collection Items => _Items ?? (_Items =
                ConfBlockItem.Collection.Create(ContentsProvider.GetConfContents(Content)));

            public object Content { get; }
            public IConfContentsProvider ContentsProvider { get; }

            public int ItemCount() => Items.Count;

            public ConfBlock(object content, IConfContentsProvider contentsProvider) {
                Content = content;
                ContentsProvider = contentsProvider;
            }

            public IConfBlockItem Item(object key) {
                if (null == key) throw new ArgumentNullException(nameof(key));

                if (key is int index) {
                    return Items[index];
                }

                var stringKey = key.ToString();
                var normalizedKey = Key.Normalize(stringKey);
                return Items[normalizedKey];
            }

            public bool ItemExists(object key, out IConfBlockItem item) {
                try {
                    item = Item(key);
                    return true;
                }
                catch {
                    item = null;
                    return false;
                }
            }

            public bool ItemExists(object key) {
                return ItemExists(key, out _);
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

        class ConfBlockItem : IConfBlockItem {
            public string OriginalKey { get; }
            public string NormalizedKey { get; }
            public string OriginalValue { get; }

            public ConfBlockItem(string originalKey, string normalizedKey, string originalValue) {
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

            public class Collection : KeyedCollection<string, IConfBlockItem> {
                void Add(IEnumerable<IConfBlockItem> items) {
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

                protected override string GetKeyForItem(IConfBlockItem item) {
                    if (null == item) throw new ArgumentNullException(nameof(item));
                    return item.NormalizedKey;
                }

                public Collection(IEnumerable<IConfBlockItem> items) {
                    Add(items);
                }

                public static Collection Create(IEnumerable<KeyValuePair<string, string>> items) {
                    if (null == items) throw new ArgumentNullException(nameof(items));
                    return new Collection(items.Select(item => new ConfBlockItem(item.Key, Key.Normalize(item.Key), item.Value)));
                }
            }
        }
    }
}
