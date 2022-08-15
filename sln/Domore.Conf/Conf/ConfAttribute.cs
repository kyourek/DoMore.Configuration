using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domore.Conf {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConfAttribute : Attribute {
        public bool Ignore { get; }
        public ReadOnlyCollection<string> Names { get; }

        public ConfAttribute(bool ignore = false, params string[] names) {
            Ignore = ignore;
            Names = new ReadOnlyCollection<string>(new List<string>(names));
        }
    }
}
