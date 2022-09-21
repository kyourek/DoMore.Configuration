using System.Collections.Generic;

namespace Domore.Conf {
    public class ConfContent {
        internal IEnumerable<IConfPair> Pairs { get; }

        internal ConfContent(params IConfPair[] pairs) {
            Pairs = new List<IConfPair>(pairs);
        }
    }
}
