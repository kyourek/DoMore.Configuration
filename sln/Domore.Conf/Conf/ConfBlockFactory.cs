using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf {
    using Helpers;
    using Providers;

    internal class ConfBlockFactory {
        public IConfBlock CreateConfBlock(object content, IConfContentsProvider contentsProvider = null, ConfConverter converter = null) {
            return new ConfBlock(content, contentsProvider ?? new ConfContentsProvider(), converter ?? new ConfConverter());
        }

        private class ConfBlock : IConfBlock {
            private ConfBlockItem.Collection Items =>
                _Items ?? (
                _Items = new ConfBlockItem.Collection(ContentsProvider.GetConfContents(Content), Converter));
            private ConfBlockItem.Collection _Items;

            public object Content { get; }
            public IConfContentsProvider ContentsProvider { get; }
            public ConfConverter Converter { get; }

            public int ItemCount() => Items.Count;
            public IEnumerable<KeyValuePair<string, string>> Contents => Items.Contents;

            public Log Log {
                get => Converter.Log;
                set => Converter.Log = value;
            }

            public ConfBlock(object content, IConfContentsProvider contentsProvider, ConfConverter converter) {
                Converter = converter;
                Content = content;
                ContentsProvider = contentsProvider ?? throw new ArgumentNullException(nameof(contentsProvider));
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

            public bool ItemExists(object key) =>
                ItemExists(key, out _);

            public object Configure(object obj, string key) {
                if (null == obj) throw new ArgumentNullException(nameof(obj));

                Log.These("Configuring", $"`{obj.GetType()}`");

                key = key ?? obj.GetType().Name;
                key = key.Trim();

                Log.These("with key", $"`{key}`");

                return Property.SetAll(obj, this, key, Converter);
            }

            public T Configure<T>(T obj, string key) =>
                (T)Configure((object)obj, key);

            public IEnumerable<T> Configure<T>(Func<T> factory, string key) =>
                Collect.All(factory, key, this);

            public override string ToString() =>
                string.Join(Environment.NewLine, Items);
        }

        private class ConfBlockItem : IConfBlockItem {
            public string OriginalKey { get; }
            public string NormalizedKey { get; }
            public string OriginalValue { get; }
            public ConfConverter Converter { get; }

            public ConfBlockItem(string originalKey, string normalizedKey, string originalValue, ConfConverter converter) {
                Converter = converter ?? throw new ArgumentNullException(nameof(converter));
                OriginalKey = originalKey;
                NormalizedKey = normalizedKey;
                OriginalValue = originalValue;
            }

            public object ConvertValue(Type type) =>
                Converter.Convert(type, OriginalValue);

            public T ConvertValue<T>() =>
                (T)ConvertValue(typeof(T));

            public override string ToString() =>
                $"{OriginalKey} = {OriginalValue}";

            public class Collection : KeyedCollection<string, IConfBlockItem> {
                private void Add(IEnumerable<IConfBlockItem> items) {
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

                public ConfConverter Converter { get; }
                public IEnumerable<KeyValuePair<string, string>> Contents { get; }

                public Collection(IEnumerable<KeyValuePair<string, string>> contents, ConfConverter converter) {
                    Contents = contents ?? throw new ArgumentNullException(nameof(contents));
                    Converter = converter;
                    Add(Contents.Select(item => new ConfBlockItem(item.Key, Key.Normalize(item.Key), item.Value, Converter)));
                }
            }
        }
    }
}
