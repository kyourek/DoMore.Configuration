using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfKeyPart {
        public string Name { get; }
        public StringComparison NameComparison { get; } = StringComparison.OrdinalIgnoreCase;
        public ReadOnlyCollection<ConfKeyIndex> Indices { get; }

        public ConfKeyPart(string name, ReadOnlyCollection<ConfKeyIndex> indices) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Indices = indices;
        }

        public ConfKeyPart(string name, params ConfKeyIndex[] indices) : this(name, new ReadOnlyCollection<ConfKeyIndex>(new List<ConfKeyIndex>(indices))) {
        }

        public bool Is(string name) {
            return Name.Equals(name, NameComparison);
        }
    }
}
