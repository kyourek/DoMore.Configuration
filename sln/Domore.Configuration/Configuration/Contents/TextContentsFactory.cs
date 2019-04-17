using System;
using System.Collections.Generic;
using System.Linq;

namespace Domore.Configuration.Contents {
    class TextContentsFactory : IConfigurationContentsFactory {
        public virtual IEnumerable<KeyValuePair<string, string>> CreateConfigurationContents(object content) {
            var contents = content?.ToString()?.Trim() ?? "";
            var separator = contents.Contains("\n") ? "\n" : ";";

            return contents
                .Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
                .Where(array => array.Length == 2)
                .Select(array => new KeyValuePair<string, string>(array[0].Trim(), array[1].Trim()));
        }
    }
}
