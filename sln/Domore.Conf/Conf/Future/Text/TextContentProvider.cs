using System.Linq;

namespace Domore.Conf.Future.Text {
    using Parsing;

    internal class TextContentProvider : IConfContentProvider {
        private readonly TokenParser Parser = new TokenParser();

        public ConfContent GetConfContent(object content) {
            var s = $"{content}";
            var d = s.Contains("\n") ? '\n' : ';';
            var p = Parser.Parse(d, s);
            var a = p.ToArray();
            var c = new ConfContent(a);
            return c;
        }
    }
}
