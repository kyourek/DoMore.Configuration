using System.Collections.Generic;

namespace Domore.Conf.Extensions {
    public static class ConfObject {
        private static readonly ConfObjectProvider Provider = new ConfObjectProvider();

        public static IEnumerable<KeyValuePair<string, string>> GetConfContents(this object @object, string key = null) {
            return Provider.GetConfContents(@object, key);
        }

        public static string GetConfText(this object @object, string key = null, bool? multiline = null) {
            return Provider.GetConfText(@object, key, multiline);
        }
    }
}
