using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("D695CFB3-BA77-4F4D-A7E8-290CBC279B77")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfigurationContainer : IConfigurationContainer {
        static readonly object StaticDefaultContentLocker = new object();

        static string _StaticDefaultContent;
        string StaticDefaultContent {
            get {
                if (_StaticDefaultContent == null) {
                    lock (StaticDefaultContentLocker) {
                        if (_StaticDefaultContent == null) {
                            _StaticDefaultContent = DefaultContent();
                        }
                    }
                }
                return _StaticDefaultContent;
            }
        }

        protected virtual string DefaultContent() {
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

        string _Content;
        public string Content {
            get {
                if (_Content == null) {
                    _Content = StaticDefaultContent;
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

        IConfigurationBlockFactory _BlockFactory;
        public IConfigurationBlockFactory BlockFactory {
            get { return _BlockFactory ?? (_BlockFactory = new ConfigurationBlockFactory()); }
            set {
                if (_BlockFactory != value) {
                    _BlockFactory = value;
                    Reset();
                }
            }
        }

        IConfigurationBlock _Block;
        public IConfigurationBlock Block {
            get { return _Block ?? (_Block = BlockFactory.CreateConfigurationBlock(Content)); }
            private set { _Block = value; }
        }

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
