using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf.Future {
    internal class ConfKeyIndex {
        public ReadOnlyCollection<ConfKeyIndexPart> Parts { get; }

        public ConfKeyIndex(ReadOnlyCollection<ConfKeyIndexPart> parts) {
            Parts = parts;
        }

        public ConfKeyIndex(params ConfKeyIndexPart[] parts) : this(new ReadOnlyCollection<ConfKeyIndexPart>(new List<ConfKeyIndexPart>(parts))) {
        }

        public ConfKeyIndex(params string[] names) : this(names?.Select(name => new ConfKeyIndexPart(name)).ToArray()) {
        }

        public override string ToString() {
            return $"{string.Join(",", Parts)}";
        }
    }
}
