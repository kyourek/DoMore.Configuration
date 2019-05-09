namespace Domore {
    using Conf;

    class Program {
        static void Main(string[] args) {
            Conf.Conf.ContentsProvider = new AppSettingsProvider();
            new Release(args);
        }
    }
}