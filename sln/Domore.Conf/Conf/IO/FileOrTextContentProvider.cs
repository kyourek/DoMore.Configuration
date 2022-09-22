using FILE = System.IO.File;

namespace Domore.Conf.IO {
    using Text;

    internal class FileOrTextContentProvider : IConfContentProvider {
        private TextContentProvider Text =>
            _Text ?? (
            _Text = new TextContentProvider());
        private TextContentProvider _Text;

        private FileContentProvider File =>
            _File ?? (
            _File = new FileContentProvider());
        private FileContentProvider _File;

        public ConfContent GetConfContent(object contents) {
            return FILE.Exists($"{contents}")
                ? File.GetConfContent(contents)
                : Text.GetConfContent(contents, null);
        }
    }
}
