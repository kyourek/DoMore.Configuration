using System;

namespace Domore.Conf.Future {
    using Text;

<<<<<<< HEAD
    public class ConfContainer : IConf {
=======
    internal class ConfContainer : IConf {
>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537
        private ConfContent Content =>
            _Content ?? (
            _Content = ContentProvider.GetConfContent(Contents));
        private ConfContent _Content;

<<<<<<< HEAD
        private ConfContentProvider ContentProvider {
            get => _ContentProvider ?? (_ContentProvider = new TextContentProvider());
=======
        private ConfPopulator Populator =>
            _Populator ?? (
            _Populator = new ConfPopulator());
        private ConfPopulator _Populator;

        public object Contents {
            get => _Contents;
>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537
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
<<<<<<< HEAD
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
=======
            var targetType = target.GetType();
            var k = key == null ? targetType.Name : key;
            var p = Content.ByKey(k);
            return target;
        }

>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537
    }
}
