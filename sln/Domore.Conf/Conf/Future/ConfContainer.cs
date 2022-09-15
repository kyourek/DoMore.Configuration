using System;

namespace Domore.Conf.Future {
    using Text;

    internal class ConfContainer : IConf {
        private ConfContent Content =>
            _Content ?? (
            _Content = ContentProvider.GetConfContent(Contents));
        private ConfContent _Content;

        private ConfPopulator Populator =>
            _Populator ?? (
            _Populator = new ConfPopulator());
        private ConfPopulator _Populator;

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

        public T Configure<T>(T target, string key = null) {
            if (null == target) throw new ArgumentNullException(nameof(target));
            var targetType = target.GetType();
            var k = key == null ? targetType.Name : key;
            var p = Content.ByKey(k);
            return target;
        }

    }
}
