using System.Collections.Generic;

namespace Domore.Conf.Future {
    internal interface IConfKeyIndex : IConfToken {
        IEnumerable<IConfKeyIndexPart> Parts { get; }
    }
}
