using System;
using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfKeyPart {
        public string Name { get; }
        public ReadOnlyCollection<ConfKeyIndex> Indices { get; }

        public ConfKeyPart(string name, ReadOnlyCollection<ConfKeyIndex> indices) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Indices = indices;
        }

        public bool Is(string name) {
            return Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
