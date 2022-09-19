namespace Domore.Conf.Future.Text {
    internal class TextContentProviderOld : ConfContentProviderOld {
        private readonly TextContentParser Parser = new TextContentParser();

        public override ConfContentOld GetConfContent(object content) {
            var s = $"{content}";
            var p = Parser.Parse(s);
            var c = ConfContentOld.From(p);
            return c;
        }
    }
}
