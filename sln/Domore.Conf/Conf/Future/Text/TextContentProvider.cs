<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
=======
﻿using System.Collections.ObjectModel;
using System.Linq;
>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537

namespace Domore.Conf.Future.Text {
    using Parsing;

    internal class TextContentProvider : ConfContentProvider {
<<<<<<< HEAD
        private readonly TokenParser Parser = new TokenParser();

        public override ConfContent GetConfContent(object content) {
            var s = $"{content}";
            var d = s.Contains("\n") ? '\n' : ';';
            var p = Parser.Parse(d, s);
            var c = new ConfContent(new ReadOnlyCollection<ConfPair>(new List<ConfPair>(p)));
            return c;
=======
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
>>>>>>> cf5b6370d0c2fac1512f79a2ea6bbddf44a5c537
        }
    }
}
