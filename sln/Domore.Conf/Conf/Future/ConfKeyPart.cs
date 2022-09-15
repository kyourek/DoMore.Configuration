using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfKeyPart {
        public ReadOnlyCollection<ConfKeyIndex> Indices { get; }

        public ConfKeyPart(ReadOnlyCollection<ConfKeyIndex> indices) {
            Indices = indices;
        }
    }
}
