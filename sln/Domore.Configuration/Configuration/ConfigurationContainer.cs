using System;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("D695CFB3-BA77-4F4D-A7E8-290CBC279B77")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfigurationContainer : IConfigurationContainer {
        ConfigurationBlockFactory _BlockFactory;
        ConfigurationBlockFactory BlockFactory {
            get => _BlockFactory ?? (_BlockFactory = new ConfigurationBlockFactory());
            set {
                if (_BlockFactory != value) {
                    _BlockFactory = value;
                    Reset();
                }
            }
        }

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

        IConfigurationContentsProvider _ContentsProvider;
        public IConfigurationContentsProvider ContentsProvider {
            get => _ContentsProvider;
            set {
                if (_ContentsProvider != value) {
                    _ContentsProvider = value;
                    Reset();
                }
            }
        }

        IConfigurationBlock _Block;
        public IConfigurationBlock Block {
            get => _Block ?? (_Block = BlockFactory.CreateConfigurationBlock(Content, ContentsProvider));
            private set => _Block = value;
        }

        public void Reset() {
            Block = null;
            OnContentsChanged(EventArgs.Empty);
        }

        public override string ToString() {
            return Content?.ToString() ?? base.ToString();
        }
    }
}
