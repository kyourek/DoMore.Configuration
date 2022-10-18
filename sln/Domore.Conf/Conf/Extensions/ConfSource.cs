namespace Domore.Conf.Extensions {
    using Text;

    public static class ConfSource {
        private static TextSourceProvider TextProvider =>
            _TextProvider ?? (
            _TextProvider = new TextSourceProvider());
        private static TextSourceProvider _TextProvider;

        public static string ConfText(this object obj, string key = null, bool? multiline = null) {
            return TextProvider.GetConfSource(obj, key, multiline);
        }
    }
}
