using System;

namespace Domore.Configuration.Helpers {
    static class Key {
        public static string Normalize(string key) {
            if (null == key) throw new ArgumentNullException(nameof(key));
            return string.Join("", key.Split())
                .ToLowerInvariant()
                .Trim();
        }
    }
}
