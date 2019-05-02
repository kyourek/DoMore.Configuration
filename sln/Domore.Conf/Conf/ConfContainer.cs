using System;
using System.Runtime.InteropServices;

namespace Domore.Conf {
    [Guid("D695CFB3-BA77-4F4D-A7E8-290CBC279B77")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConfContainer : IConfContainer {
        ConfBlockFactory _BlockFactory;
        ConfBlockFactory BlockFactory {
            get => _BlockFactory ?? (_BlockFactory = new ConfBlockFactory());
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

        IConfContentsProvider _ContentsProvider;
        public IConfContentsProvider ContentsProvider {
            get => _ContentsProvider;
            set {
                if (_ContentsProvider != value) {
                    _ContentsProvider = value;
                    Reset();
                }
            }
        }

        IConfBlock _Block;
        public IConfBlock Block {
            get => _Block ?? (_Block = BlockFactory.CreateConfBlock(Content, ContentsProvider));
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
