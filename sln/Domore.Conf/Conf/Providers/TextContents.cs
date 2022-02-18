namespace Domore.Conf.Providers.TextContents {
    internal static class StringExtension {
        public static bool IsWhiteSpace(this string s) {
            return s == null
                ? false
                : string.IsNullOrWhiteSpace(s);
        }

        public static bool IsCharAndWhiteSpace(this string s, char c) {
            if (s == null) return false;
            var found = false;
            var length = s.Length;
            for (var i = 0; i < length; i++) {
                var si = s[i];
                var whiteSpace = char.IsWhiteSpace(si);
                if (whiteSpace == false) {
                    if (found == true) {
                        return false;
                    }
                    if (si != c) {
                        return false;
                    }
                    else {
                        found = true;
                    }
                }
            }
            return found;
        }
    }
}
