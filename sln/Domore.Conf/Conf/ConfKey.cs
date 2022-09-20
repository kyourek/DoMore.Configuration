using System;

namespace Domore.Conf {
    internal static class ConfKey {
        public static bool StartsWith(this IConfKey confKey, string key) {
            if (null == confKey) throw new ArgumentNullException(nameof(confKey));

            var parts = confKey.Parts;
            if (parts.Count < 1) return false;

            var first = parts[0];
            if (first == null) return false;

            var name = first.Content;
            if (name == null) return false;

            return name.Equals(key, StringComparison.OrdinalIgnoreCase);
        }
    }
}
