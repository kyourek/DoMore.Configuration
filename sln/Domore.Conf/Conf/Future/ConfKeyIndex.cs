using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfKeyIndex {
        public ReadOnlyCollection<ConfKeyIndexPart> Parts { get; }

        public ConfKeyIndex(ReadOnlyCollection<ConfKeyIndexPart> parts) {
            Parts = parts;
        }
    }
}
