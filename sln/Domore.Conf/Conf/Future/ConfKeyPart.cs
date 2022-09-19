using System;
<<<<<<< HEAD
using System.Collections.Generic;
=======
>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537
using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfKeyPart {
        public string Name { get; }
<<<<<<< HEAD
        public StringComparison NameComparison { get; } = StringComparison.OrdinalIgnoreCase;
=======
>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537
        public ReadOnlyCollection<ConfKeyIndex> Indices { get; }

        public ConfKeyPart(string name, ReadOnlyCollection<ConfKeyIndex> indices) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Indices = indices;
        }

<<<<<<< HEAD
        public ConfKeyPart(string name, params ConfKeyIndex[] indices) : this(name, new ReadOnlyCollection<ConfKeyIndex>(new List<ConfKeyIndex>(indices))) {
        }

        public bool Is(string name) {
            return Name.Equals(name, NameComparison);
=======
        public bool Is(string name) {
            return Name.Equals(name, StringComparison.OrdinalIgnoreCase);
>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537
        }
    }
}
