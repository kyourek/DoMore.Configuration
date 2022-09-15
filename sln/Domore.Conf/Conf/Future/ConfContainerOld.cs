using System;
using System.Collections.Generic;

namespace Domore.Conf.Future {
    using Text;

    public class ConfContainerOld {
        private ConfContentOld Content =>
            _Content ?? (
            _Content = ContentProvider.GetConfContent(Contents));
        private ConfContentOld _Content;

        public object Contents {
            get => _Contents;
            set {
                _Contents = value;
                _Content = null;
            }
        }
        private object _Contents;

        public ConfContentProviderOld ContentProvider {
            get => _ContentProvider ?? (_ContentProvider = new TextContentProviderOld());
            set {
                _ContentProvider = value;
                _Content = null;
            }
        }
        private ConfContentProviderOld _ContentProvider;

        public T Configure<T>(T obj, string key = null) {
            return Content.Configure(obj, key);
        }

        public IEnumerable<T> Configure<T>(Func<T> factory, string key = null) {
            return Content.Configure(factory, key);
        }
    }
}
