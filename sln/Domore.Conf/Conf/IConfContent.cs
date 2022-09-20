using System.Collections.Generic;

namespace Domore.Conf {
    public interface IConfContent {
        IEnumerable<IConfPair> Pairs { get; }
    }
}
