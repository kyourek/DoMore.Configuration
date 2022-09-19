using System;
<<<<<<< HEAD
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
=======
using System.Collections.ObjectModel;
>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537

namespace Domore.Conf.Future {
    internal class ConfKey {
        public ReadOnlyCollection<ConfKeyPart> Parts { get; }

        public ConfKey(ReadOnlyCollection<ConfKeyPart> parts) {
            Parts = parts ?? throw new ArgumentNullException(nameof(parts));
        }

<<<<<<< HEAD
        public ConfKey(params ConfKeyPart[] parts) : this(new ReadOnlyCollection<ConfKeyPart>(new List<ConfKeyPart>(parts))) {
        }

        public bool StartsWith(string name) {
            var parts = Parts;
            if (parts.Count > 0) {
                var first = parts[0];
                if (first.Is(name)) {
=======
        public bool StartsWith(string key) {
            if (Parts.Count > 0) {
                if (Parts[0].Is(key)) {
>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537
                    return true;
                }
            }
            return false;
<<<<<<< HEAD
        }

        public ConfKey Skip(int parts) {
            return new ConfKey(
                parts: new ReadOnlyCollection<ConfKeyPart>(Parts.Skip(parts).ToList()));
=======
>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537
        }
    }
}
