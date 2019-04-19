namespace Domore {
    using Configuration;

    class Program {
        static void Main() {
            ConfigurationDefault.ContentsProvider = new AppSettingsProvider();
            new Sample().Run();
        }
    }
}
