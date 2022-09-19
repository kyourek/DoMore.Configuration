using System;
using System.Collections.Generic;

namespace Domore.Conf.Future {
    using Text;

    public class ConfContainer : IConf {
        private ConfContent Content =>
            _Content ?? (
            _Content = ContentProvider.GetConfContent(Contents));
        private ConfContent _Content;

        private ConfContentProvider ContentProvider {
            get => _ContentProvider ?? (_ContentProvider = new TextContentProvider());
            set {
                _ContentProvider = value;
                _Content = null;
            }
        }
        private ConfContentProvider _ContentProvider;

        public object Contents {
            get => _Contents;
            set {
                _Contents = value;
                _Content = null;
            }
        }
        private object _Contents;

        public T Configure<T>(T target, string key = null) {
            if (null == target) throw new ArgumentNullException(nameof(target));
            var k = key ?? typeof(T).Name;
            var p = k == "" ? Content.Pairs : Content.PairsOf(k);
            new ConfPopulator().Populate(target, this, p);
            return target;
        }

        public IEnumerable<T> Configure<T>(Func<T> factory, string key = null) {
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            var k = key ?? typeof(T).Name;
            var p = k == "" ? Content.Pairs : Content.PairsOf(k);
            var i = factory();
            new ConfPopulator().Populate(i, this, p);
            yield return i;
        }
    }
}
