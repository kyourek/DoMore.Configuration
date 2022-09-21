namespace Domore.Conf.Extensions {
    using Text;

    internal static class ConfSource {
        private static TextSourceProvider TextProvider =>
            _TextProvider ?? (
            _TextProvider = new TextSourceProvider());
        private static TextSourceProvider _TextProvider;

        public static string GetConfText(this object obj, string key = null, bool? multiline = null) {
            return TextProvider.GetConfSource(obj, key, multiline);
        }
    }
}
