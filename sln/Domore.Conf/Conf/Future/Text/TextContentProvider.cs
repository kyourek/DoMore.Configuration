using System.Collections.ObjectModel;
using System.Linq;

namespace Domore.Conf.Future.Text {
    using Parsing;

    internal class TextContentProvider : ConfContentProvider {
        private MultiLineParser MultiLineParser =>
            _MultiLineParser ?? (
            _MultiLineParser = new MultiLineParser());
        private MultiLineParser _MultiLineParser;

        public override ConfContent GetConfContent(object contents) {
            var text = contents?.ToString() ?? "";
            var parser = MultiLineParser;
            var pairs = parser.Parse(text);
            var list = pairs.ToList();
            var conf = new ReadOnlyCollection<ConfPair>(list);
            var content = new ConfContent(conf);
            return content;
        }
    }
}
