using System;
using System.Collections.Generic;

namespace Domore.Conf.Future {
    using Text;

    public class ConfContainer {
        private ConfContent Content =>
            _Content ?? (
            _Content = ContentProvider.GetConfContent(Contents));
        private ConfContent _Content;

        public object Contents {
            get => _Contents;
            set {
                _Contents = value;
                _Content = null;
            }
        }
        private object _Contents;

        public ConfContentProvider ContentProvider {
            get => _ContentProvider ?? (_ContentProvider = new TextContentProvider());
            set {
                _ContentProvider = value;
                _Content = null;
            }
        }
        private ConfContentProvider _ContentProvider;

        public T Configure<T>(T obj, string key = null) {
            return Content.Configure(obj, key);
        }

        public IEnumerable<T> Configure<T>(Func<T> factory, string key = null) {
            return Content.Configure(factory, key);
        }
    }
}
