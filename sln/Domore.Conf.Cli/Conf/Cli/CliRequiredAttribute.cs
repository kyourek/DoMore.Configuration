using System;

namespace Domore.Conf.Cli {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CliRequiredAttribute : Attribute {
    }
}
