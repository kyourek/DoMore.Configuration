using System;

namespace Domore.Conf.Helpers {
    class Log {
        public ConfLog Action { get; set; }

        public void Lines(params object[] values) {
            Action?.Invoke(string.Join(Environment.NewLine, values) + Environment.NewLine);
        }
    }
}
