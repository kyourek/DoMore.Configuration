using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domore.Conf.Future.Text {
    using Parsing;

    internal class TextContentProvider : ConfContentProvider {
        private readonly TokenParser Parser = new TokenParser();

        public override ConfContent GetConfContent(object content) {
            var s = $"{content}";
            var d = s.Contains("\n") ? '\n' : ';';
            var p = Parser.Parse(d, s);
            var c = new ConfContent(new ReadOnlyCollection<ConfPair>(new List<ConfPair>(p)));
            return c;
        }
    }
}
