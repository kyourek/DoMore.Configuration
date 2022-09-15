using System;
using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfKey {
        public ReadOnlyCollection<ConfKeyPart> Parts { get; }

        public ConfKey(ReadOnlyCollection<ConfKeyPart> parts) {
            Parts = parts ?? throw new ArgumentNullException(nameof(parts));
        }

        public bool StartsWith(string key) {
            if (Parts.Count > 0) {
                if (Parts[0].Is(key)) {
                    return true;
                }
            }
            return false;
        }
    }
}
