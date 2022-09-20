using System.IO;

namespace Domore.Conf.IO {
    using Text;

    internal class FileContentProvider : IConfContentProvider {
        private TextContentProvider Text =>
            _Text ?? (
            _Text = new TextContentProvider());
        private TextContentProvider _Text;

        public IConfContent GetConfContent(object contents) {
            var path = $"{contents}";
            var text = File.ReadAllText(path);
            return Text.GetConfContent(text);
        }
    }
}
