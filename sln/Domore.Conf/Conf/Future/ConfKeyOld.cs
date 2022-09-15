using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domore.Conf.Future {
    internal class ConfKeyOld {
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

        private static string Normalize(string key) {
            if (null == key) throw new ArgumentNullException(nameof(key));
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

        public string Original { get; }

        public string Normalized =>
            _Normalized ?? (
            _Normalized = Normalize(Original));
        private string _Normalized;

        public ReadOnlyCollection<string> Parts =>
            _Parts ?? (
            _Parts = new ReadOnlyCollection<string>(Normalized.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)));
        private ReadOnlyCollection<string> _Parts;

        public ConfKeyOld(string original) {
            Original = original ?? throw new ArgumentNullException(nameof(original));
        }

        public bool StartsWith(ConfKeyOld other) {
            if (null == other) throw new ArgumentNullException(nameof(other));
            var thisParts = Parts;
            var otherParts = other.Parts;
            if (otherParts.Count > thisParts.Count) {
                return false;
            }
            for (var i = 0; i < otherParts.Count; i++) {
                if (otherParts[i] != thisParts[i]) {
                    return false;
                }
            }
            return true;
        }

        public class Comparer : IEqualityComparer<ConfKeyOld> {
            public static bool Equals(ConfKeyOld x, ConfKeyOld y) {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                if (x.Normalized == y.Normalized) return true;
                return false;
            }

            public static int GetHashCode(ConfKeyOld obj) {
                return obj == null
                    ? 0
                    : obj.Normalized.GetHashCode();
            }

            bool IEqualityComparer<ConfKeyOld>.Equals(ConfKeyOld x, ConfKeyOld y) {
                return Equals(x, y);
            }

            int IEqualityComparer<ConfKeyOld>.GetHashCode(ConfKeyOld obj) {
                return GetHashCode(obj);
            }
        }
    }
}
