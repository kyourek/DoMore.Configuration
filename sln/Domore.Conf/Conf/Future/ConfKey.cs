using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf.Future {
    internal class ConfKey {
        public ReadOnlyCollection<ConfKeyPart> Parts { get; }

        public ConfKey(ReadOnlyCollection<ConfKeyPart> parts) {
            Parts = parts ?? throw new ArgumentNullException(nameof(parts));
        }

        public ConfKey(params ConfKeyPart[] parts) : this(new ReadOnlyCollection<ConfKeyPart>(new List<ConfKeyPart>(parts))) {
        }

        public bool StartsWith(string name) {
            var parts = Parts;
            if (parts.Count > 0) {
                var first = parts[0];
                if (first.Is(name)) {
                    return true;
                }
            }
            return false;
        }

        public ConfKey Skip(int parts) {
            return new ConfKey(
                parts: new ReadOnlyCollection<ConfKeyPart>(Parts.Skip(parts).ToList()));
        }

        public override string ToString() {
            return string.Join(".", Parts);
        }
    }
}
