using System;

namespace Domore.Conf {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConfAttribute : Attribute {
        public bool Ignore { get; set; }
    }
}
