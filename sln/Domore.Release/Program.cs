using System;

namespace Domore {
    using Conf;
    using CONF = Conf.Conf;

    class Program {
        static void Main(string[] args) {
            try {
                CONF.Container.ContentsProvider = new AppSettingsProvider();
                CONF.Container.ConfigureLogging();

                var command = ReleaseCommand.From(args);
                new Release(command);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
    }
}