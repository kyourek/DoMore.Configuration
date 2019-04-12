using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    using Helpers;

    [Guid("7506691D-E25D-4D44-BBEF-5CDE862E9148")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfigurationBlockFactory : IConfigurationBlockFactory {
        public IConfigurationBlock CreateConfigurationBlock(string configuration) {
            return new ConfigurationBlock(configuration);
        }

        private class ConfigurationBlock : IConfigurationBlock {
            private ConfigurationBlockItem.Collection Items {
                get { return _Items ?? (_Items = CreateItems(ConfigurationContent)); }
            }
            private ConfigurationBlockItem.Collection _Items;

            private static string GetConfigurationContent(string configuration) {
                if (configuration == null) {
                    return string.Empty;
                }
                if (File.Exists(configuration)) {
                    return File.ReadAllText(configuration);
                }
                return configuration;
            }

            private static ConfigurationBlockItem.Collection CreateItems(string configurationContent) {
                if (null == configurationContent) throw new ArgumentNullException(nameof(configurationContent));

                var content = configurationContent.Trim();
                var separator = content.Contains("\n") 
                    ? "\n" 
                    : ";";

                return new ConfigurationBlockItem.Collection(
                    content
                        .Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .Select(line => line.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
                        .Where(array => array.Length == 2)
                        .Select(array => new {
                            Key = array[0].Trim(),
                            Value = array[1].Trim()
                        })
                        .Select(item => new ConfigurationBlockItem(item.Key, Key.Normalize(item.Key), item.Value)));
            }

            public string Configuration { get; }

            public string ConfigurationContent {
                get { return _ConfigurationContent ?? (_ConfigurationContent = GetConfigurationContent(Configuration)); }
            }
            private string _ConfigurationContent;

            public int ItemCount {
                get { return Items.Count; }
            }

            public ConfigurationBlock(string configuration) {
                Configuration = configuration;
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

                if (key is int) {
                    var index = (int)key;
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
                return Configuration;
            }
        }

        private class ConfigurationBlockItem : IConfigurationBlockItem {
            public string OriginalKey { get; }
            public string NormalizedKey { get; }
            public string OriginalValue { get; }

            public string StringValue {
                get { return OriginalValue; }
            }

            public int IntegerValue {
                get { return _IntegerValue ?? (_IntegerValue = int.Parse(OriginalValue)).Value; }
            }
            private int? _IntegerValue;

            public double NumberValue {
                get { return _NumberValue ?? (_NumberValue = double.Parse(OriginalValue)).Value; }
            }
            private double? _NumberValue;

            public bool BooleanValue {
                get { return _BooleanValue ?? (_BooleanValue = bool.Parse(OriginalValue)).Value; }
            }
            private bool? _BooleanValue;

            public ConfigurationBlockItem(string originalKey, string normalizedKey, string originalValue) {
                OriginalKey = originalKey;
                NormalizedKey = normalizedKey;
                OriginalValue = originalValue;
            }

            public override string ToString() {
                return string.Join(" = ", OriginalKey, OriginalValue);
            }

            public class Collection : KeyedCollection<string, ConfigurationBlockItem> {
                private void Add(IEnumerable<ConfigurationBlockItem> items) {
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

                protected override string GetKeyForItem(ConfigurationBlockItem item) {
                    if (null == item) throw new ArgumentNullException(nameof(item));
                    return item.NormalizedKey;
                }

                public Collection(IEnumerable<ConfigurationBlockItem> items) {
                    Add(items);
                }
            }
        }
    }
}
