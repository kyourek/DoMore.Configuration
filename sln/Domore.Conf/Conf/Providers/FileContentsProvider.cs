using System.Collections.Generic;
using System.IO;

namespace Domore.Conf.Providers {
    internal class FileContentsProvider : TextContentsProvider {
        public override IEnumerable<KeyValuePair<string, string>> GetConfContents(object content) {
            var file = content?.ToString();
            if (File.Exists(file)) content = File.ReadAllText(file);
            return base.GetConfContents(content);
        }
    }
}
