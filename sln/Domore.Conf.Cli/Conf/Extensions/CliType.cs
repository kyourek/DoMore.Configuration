using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domore.Conf.Extensions {
    using Cli;

    internal static class CliType {
        public static Dictionary<MemberInfo, string> GetEnumDisplay(this Type type) {
            var alias = ConfType.GetEnumAlias(type);
            var display = ConfType.GetEnumAttributes<CliDisplayAttribute>(type).ToDictionary(
                pair => pair.Key,
                pair => pair.Value?.Override ?? alias[pair.Key].OrderBy(a => a.Length).First());
            return display;
        }
    }
}
