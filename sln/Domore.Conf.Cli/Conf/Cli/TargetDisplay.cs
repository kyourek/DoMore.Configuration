using System;
using System.Linq;

namespace Domore.Conf.Cli {
    internal static class TargetDisplay {
        public static string For(TargetDescription target) {
            if (null == target) throw new ArgumentNullException(nameof(target));
            var propertyDisplayDefault = target.DisplayDefault;
            var propertyDisplays = target.Properties
                .Where(p => p.DisplayAttribute.Include ?? propertyDisplayDefault)
                .Select(p => p.Display);
            var properties = string.Join(" ", propertyDisplays);
            return string.Join(" ", target.CommandName, properties);
        }
    }
}
