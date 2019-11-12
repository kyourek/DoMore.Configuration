namespace Domore.Conf.Helpers {
    internal class Log {
        public ConfLog Action { get; set; }

        public void These(params object[] values) =>
            Action?.Invoke(string.Join(" ", values));
    }
}
