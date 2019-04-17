using System.Collections.Generic;
using System.IO;

namespace Domore.Configuration.Contents {
    class FileContentsFactory : TextContentsFactory {
        public override IEnumerable<KeyValuePair<string, string>> CreateConfigurationContents(object content) {
            var file = content?.ToString();
            if (File.Exists(file)) content = File.ReadAllText(file);
            return base.CreateConfigurationContents(content);
        }
    }
}
