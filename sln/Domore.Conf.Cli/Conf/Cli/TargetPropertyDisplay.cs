using System;

namespace Domore.Conf.Cli {
    internal static class TargetPropertyDisplay {
        public static string For(TargetPropertyDescription target) {
            if (null == target) throw new ArgumentNullException(nameof(target));
            var required = target.Required;
            var propertyType = target.PropertyType;
            var argumentName = target.ArgumentName;
            if (argumentName != "" && (propertyType == typeof(string) || propertyType == typeof(object))) {
                var arg = '<' + argumentName + '>';
                return required
                    ? arg
                    : ('[' + arg + ']');
            }
            var key = argumentName;
            if (key == "") {
                key = target.DisplayName + '=';
            }
            var val = '<' + target.DisplayKind + '>';
            var pair = key + val;
            return required
                ? pair
                : ('[' + pair + ']');
        }
    }
}
