using System.Collections.Generic;
using System.Linq;

namespace Domore.Conf.Text {
    using Parsing;

    public class TextContentProvider : IConfContentProvider {
        private readonly TokenParser Parser = new TokenParser();

        public ConfContent GetConfContent(object source, IEnumerable<object> sources) {
            var s = $"{source}";
            var d = s.Contains("\n") ? '\n' : ';';
            var p = Parser.Parse(d, s);
            var a = p.ToArray();
            var c = new ConfContent(
                pairs: p.ToList(),
                sources: sources?
                    .Concat(new[] { s })?
                    .ToArray() ?? new[] { s });
            return c;
        }

        ConfContent IConfContentProvider.GetConfContent(object source) {
            return GetConfContent(source, null);
        }
    }
}
