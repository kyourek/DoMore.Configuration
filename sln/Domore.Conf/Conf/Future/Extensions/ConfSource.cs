namespace Domore.Conf.Future.Extensions {
    using Text.Reversing;

    internal static class ConfSource {
        private static Reverse Reverse =>
            _Reverse ?? (
            _Reverse = new Reverse());
        private static Reverse _Reverse;

        public static string GetConfText(this object target, string key = null, bool? multiline = null) {
            return Reverse.ConfText(target, key, multiline);
        }
    }
}
