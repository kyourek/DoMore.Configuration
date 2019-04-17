namespace Domore {
    using Configuration;

    class Program {
        static void Main() {
            ConfigurationDefault.ContentsFactory = new AppSettingsFactory();
            new Sample().Run();
        }
    }
}
