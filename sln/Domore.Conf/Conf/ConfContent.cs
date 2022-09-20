using System.Collections.Generic;

namespace Domore.Conf {
    internal class ConfContent : IConfContent {
        public IEnumerable<IConfPair> Pairs { get; }

        public ConfContent(params IConfPair[] pairs) {
            Pairs = new List<IConfPair>(pairs);
        }
    }
}
