using System.Collections.Generic;

namespace Domore.Conf {
    internal interface IConfKeyIndex : IConfToken {
        IEnumerable<IConfKeyIndexPart> Parts { get; }
    }
}
