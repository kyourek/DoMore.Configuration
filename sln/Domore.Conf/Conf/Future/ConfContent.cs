using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf.Future {
    internal class ConfContent {
        public ReadOnlyCollection<ConfPair> Conf { get; }

        public ConfContent(ReadOnlyCollection<ConfPair> conf) {
            Conf = conf ?? throw new ArgumentNullException(nameof(conf));
        }

        public IEnumerable<ConfPair> ByKey(string key) {
            return string.IsNullOrWhiteSpace(key)
                ? Conf
                : Conf.Where(c => c.StartsWith(key));
        }
    }
}
