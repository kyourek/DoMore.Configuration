using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domore.Conf {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConfAttribute : Attribute {
        private ConfAttribute(bool ignore, string[] names) {
            Ignore = ignore;
            Names = new ReadOnlyCollection<string>(new List<string>(names ?? new string[] { }));
        }

        public bool Ignore { get; }
        public ReadOnlyCollection<string> Names { get; }

        public ConfAttribute(bool ignore) : this(ignore: ignore, names: null) {
        }

        public ConfAttribute(params string[] names) : this(ignore: false, names: names) {
        }
    }
}
