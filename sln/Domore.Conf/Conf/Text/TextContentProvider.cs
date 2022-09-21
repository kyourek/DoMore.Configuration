﻿using System.Linq;

namespace Domore.Conf.Text {
    using Domore.Conf;
    using Parsing;

    public class TextContentProvider : IConfContentProvider {
        private readonly TokenParser Parser = new TokenParser();

        public IConfContent GetConfContent(object source) {
            var s = $"{source}";
            var d = s.Contains("\n") ? '\n' : ';';
            var p = Parser.Parse(d, s);
            var a = p.ToArray();
            var c = new ConfContent(a);
            return c;
        }
    }
}
