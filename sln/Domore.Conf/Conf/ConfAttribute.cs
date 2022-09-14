using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domore.Conf {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConfAttribute : Attribute {
        private ConfAttribute(bool ignore, bool ignoreGet, bool ignoreSet, Type converter, string[] names) {
            Ignore = ignore;
            IgnoreGet = ignoreGet;
            IgnoreSet = ignoreSet;
            Converter = converter;
            Names = new ReadOnlyCollection<string>(new List<string>(names ?? new string[] { }));
        }

        public bool Ignore { get; }
        public bool IgnoreGet { get; }
        public bool IgnoreSet { get; }
        public Type Converter { get; }
        public ReadOnlyCollection<string> Names { get; }

        public ConfAttribute(bool ignore) : this(ignore: ignore, ignoreGet: ignore, ignoreSet: ignore, converter: null, names: null) {
        }

        public ConfAttribute(bool ignoreGet, bool ignoreSet) : this(ignore: ignoreGet && ignoreSet, ignoreGet: ignoreGet, ignoreSet: ignoreSet, converter: null, names: null) {
        }

        public ConfAttribute(Type converter, params string[] names) : this(ignore: false, ignoreGet: false, ignoreSet: false, converter: converter, names: names) {
        }

        public ConfAttribute(params string[] names) : this(converter: null, names: names) {
        }
    }
}
