using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    using Helpers;

    [Guid("D695CFB3-BA77-4F4D-A7E8-290CBC279B77")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfigurationContainer : IConfigurationContainer {
        private static readonly object DefaultContentLocker = new object();

        private static string _DefaultContent;
        private string DefaultContent {
            get {
                if (_DefaultContent == null) {
                    lock (DefaultContentLocker) {
                        if (_DefaultContent == null) {
                            _DefaultContent = GetDefaultContent();
                        }
                    }
                }
                return _DefaultContent;
            }
        }

        protected virtual string GetDefaultContent() {
            var proc = Process.GetCurrentProcess();
            var procFile = proc.MainModule.FileName;
            var confFile = Path.ChangeExtension(procFile, ".conf");
            if (confFile.Contains(".vshost") && File.Exists(confFile) == false) {
                confFile = confFile.Replace(".vshost", "");
            }
            if (File.Exists(confFile)) {
                return confFile;
            }
            var confFileDefault = confFile + ".default";
            if (File.Exists(confFileDefault)) {
                File.Copy(confFileDefault, confFile);
                return confFile;
            }
            return "";
        }

        protected virtual void OnChanged(EventArgs e) {
            var handler = Changed;
            if (handler != null) handler.Invoke(this, e);
        }

        public event EventHandler Changed;

        public string Content {
            get {
                if (_Content == null) {
                    _Content = DefaultContent;
                }
                return _Content;
            }
            set {
                if (_Content != value) {
                    _Content = value;
                    Reset();
                }
            }
        }
        private string _Content;

        public IConfigurationBlockFactory BlockFactory {
            get { return _BlockFactory ?? (_BlockFactory = new ConfigurationBlockFactory()); }
            set {
                if (_BlockFactory != value) {
                    _BlockFactory = value;
                    Reset();
                }
            }
        }
        private IConfigurationBlockFactory _BlockFactory;

        public IConfigurationBlock Block {
            get { return _Block ?? (_Block = BlockFactory.CreateConfigurationBlock(Content)); }
            private set { _Block = value; }
        }
        private IConfigurationBlock _Block;

        public void Reset() {
            Block = null;
            OnChanged(EventArgs.Empty);
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

        public object Configure(object obj, string key) {
            return Block.Configure(obj, key);
        }

        public T Configure<T>(T obj, string key) {
            return Block.Configure(obj, key);
        }

        public override string ToString() {
            return Content;
        }
    }
}
