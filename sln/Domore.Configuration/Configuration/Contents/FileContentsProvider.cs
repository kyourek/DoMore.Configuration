using System.Collections.Generic;
using System.IO;

namespace Domore.Configuration.Contents {
    class FileContentsProvider : TextContentsProvider {
        public override IEnumerable<KeyValuePair<string, string>> GetConfigurationContents(object content) {
            var file = content?.ToString();
            if (File.Exists(file)) content = File.ReadAllText(file);
            return base.GetConfigurationContents(content);
        }
    }
}
