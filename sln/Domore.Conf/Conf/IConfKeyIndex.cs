using System.Collections.Generic;

namespace Domore.Conf {
    public interface IConfKeyIndex : IConfToken {
        IEnumerable<IConfKeyIndexPart> Parts { get; }
    }
}
