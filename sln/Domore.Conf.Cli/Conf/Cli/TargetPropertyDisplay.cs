using System;

namespace Domore.Conf.Cli {
    internal static class TargetPropertyDisplay {
        public static string For(TargetPropertyDescription target) {
            if (null == target) throw new ArgumentNullException(nameof(target));
            var key = target.ArgumentName == ""
                ? (target.DisplayName + '=')
                : (target.ArgumentName);
            var val = '<' + target.DisplayKind + '>';
            if (target.Required) {
                return key + val;
            }
            return '[' + key + val + ']';
        }
    }
}
