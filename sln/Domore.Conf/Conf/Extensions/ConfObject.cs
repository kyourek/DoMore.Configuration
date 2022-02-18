using System.Collections.Generic;

namespace Domore.Conf.Extensions {
    using Providers;

    public static class ConfObject {
        private static readonly TextContentsProvider.Reverse Reverse = new TextContentsProvider.Reverse();

        public static IEnumerable<KeyValuePair<string, string>> GetConfContents(this object @object, string key = null) {
            return Reverse.ConfContents(@object, key);
        }

        public static string GetConfText(this object @object, string key = null, bool? multiline = null) {
            return Reverse.ConfText(@object, key, multiline);
        }
    }
}
