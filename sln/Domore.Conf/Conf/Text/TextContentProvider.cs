using System.Linq;

namespace Domore.Conf.Text {
    using Domore.Conf;
    using Parsing;

    public class TextContentProvider : IConfContentProvider {
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
