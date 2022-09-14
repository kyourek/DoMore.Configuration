namespace Domore.Conf.Future.Text {
    internal class TextContentProvider : ConfContentProvider {
        private readonly TextContentParser Parser = new TextContentParser();

        public override ConfContent GetConfContent(object content) {
            var s = $"{content}";
            var p = Parser.Parse(s);
            var c = ConfContent.From(p);
            return c;
        }
    }
}
