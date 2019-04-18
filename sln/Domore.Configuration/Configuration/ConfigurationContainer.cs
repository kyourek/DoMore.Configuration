using System;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("D695CFB3-BA77-4F4D-A7E8-290CBC279B77")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfigurationContainer : IConfigurationContainer {
        protected virtual void OnContentsChanged(EventArgs e) {
            var handler = ContentsChanged;
            if (handler != null) handler.Invoke(this, e);
        }

        public event EventHandler ContentsChanged;

        object _Content;
        public object Content {
            get => _Content;
            set {
                if (_Content != value) {
                    _Content = value;
                    Reset();
                }
            }
        }

        IConfigurationContentsFactory _ContentsFactory;
        public IConfigurationContentsFactory ContentsFactory {
            get => _ContentsFactory;
            set {
                if (_ContentsFactory != value) {
                    _ContentsFactory = value;
                    Reset();
                }
            }
        }

        IConfigurationBlockFactory _BlockFactory;
        public IConfigurationBlockFactory BlockFactory {
            get => _BlockFactory ?? (_BlockFactory = new ConfigurationBlockFactory());
            set {
                if (_BlockFactory != value) {
                    _BlockFactory = value;
                    Reset();
                }
            }
        }

        IConfigurationBlock _Block;
        public IConfigurationBlock Block {
            get => _Block ?? (_Block = BlockFactory.CreateConfigurationBlock(Content, ContentsFactory));
            private set => _Block = value;
        }

        public void Reset() {
            Block = null;
            OnContentsChanged(EventArgs.Empty);
        }

        public string Value(object key) {
            return Block.Item(key).OriginalValue;
        }

        public T Value<T>(object key) {
            return (T)Convert.ChangeType(Value(key), typeof(T));
        }

        public T Value<T>(object key, T def) {
            return Block.ItemExists(key)
                ? Value<T>(key)
                : def;
        }

        public T Value<T>(object key, out T value) {
            return value = Value<T>(key);
        }

        public T Value<T>(object key, out T value, T def) {
            return value = Value<T>(key, def);
        }

        public object Configure(object obj, string key = null) {
            return Block.Configure(obj, key);
        }

        public T Configure<T>(T obj, string key = null) {
            return Block.Configure(obj, key);
        }

        public override string ToString() {
            return Content?.ToString() ?? base.ToString();
        }
    }
}
