using System;
using System.Runtime.InteropServices;

namespace Domore.Conf {
    using Providers;

    [Guid("D695CFB3-BA77-4F4D-A7E8-290CBC279B77")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfContainer : IConfContainer {
        private ConfBlockFactory BlockFactory {
            get => _BlockFactory ?? (_BlockFactory = new ConfBlockFactory());
            set {
                if (_BlockFactory != value) {
                    _BlockFactory = value;
                    Reset();
                }
            }
        }
        private ConfBlockFactory _BlockFactory;

        private ConfConverter Converter =>
            _Converter ?? (
            _Converter = new ConfConverter());
        private ConfConverter _Converter;

        protected virtual void OnContentsChanged(EventArgs e) {
            var handler = ContentsChanged;
            if (handler != null) handler.Invoke(this, e);
        }

        public event EventHandler ContentsChanged;

        public IConfConverterTable TypeConverter => Converter;

        public ConfLog Log {
            get => Converter.Log.Action;
            set => Converter.Log.Action = value;
        }

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

        public IConfContentsProvider ContentsProvider {
            get => _ContentsProvider ?? (_ContentsProvider = new ConfContentsProvider());
            set {
                if (_ContentsProvider != value) {
                    _ContentsProvider = value;
                    Reset();
                }
            }
        }
        private IConfContentsProvider _ContentsProvider;

        public IConfBlock Block {
            get => _Block ?? (_Block = BlockFactory.CreateConfBlock(Content, ContentsProvider, Converter));
            private set => _Block = value;
        }
        private IConfBlock _Block;

        public void Reset() {
            Block = null;
            OnContentsChanged(EventArgs.Empty);
        }

        public override string ToString() {
            return Content?.ToString() ?? base.ToString();
        }
    }
}
