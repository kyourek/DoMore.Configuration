using System;

namespace Domore.Conf.Future {
    internal class ConfKeyIndexPart {
        public string Name { get; }

        public ConfKeyIndexPart(string name) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string ToString() {
            return Name;
        }

        public override bool Equals(object obj) {
            return
                obj is ConfKeyIndexPart other &&
                other.Name.Equals(Name);
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }
}
