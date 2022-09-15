using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfKey {
        public ReadOnlyCollection<ConfKeyPart> Parts { get; }

        public ConfKey(ReadOnlyCollection<ConfKeyPart> parts) {
            Parts = parts;
        }
    }
}
