using System;

namespace Domore.Conf.Helpers {
    internal static class Key {
        private static string NormalizeName(string s) {
            if (null == s) throw new ArgumentNullException(nameof(s));
            return string.Join("", s.Split()).ToLowerInvariant().Trim();
        }

        private static string NormalizeIndex(string s) {
            if (null == s) throw new ArgumentNullException(nameof(s));
            return s.Trim();
        }

        private static bool ContainsIndex(string s) {
            if (null == s) throw new ArgumentNullException(nameof(s));
            var open = s.IndexOf('[');
            var close = s.IndexOf(']');
            return open >= 0 && close > open;
        }

        public static string Normalize(string key) {
            if (ContainsIndex(key)) {
                var cpy = key;
                key = "";
                do {
                    var nam = cpy.Substring(0, cpy.IndexOf('['));
                    var len = nam.Length + 1;
                    var idx = cpy.Substring(len, cpy.IndexOf(']') - len);
                    key = $"{key}{NormalizeName(nam)}[{NormalizeIndex(idx)}]";
                    cpy = cpy.Substring(cpy.IndexOf(']') + 1);
                } while (ContainsIndex(cpy));
                key = $"{key}{NormalizeName(cpy)}";
            }
            else {
                key = NormalizeName(key);
            }
            return key;
        }
    }
}
