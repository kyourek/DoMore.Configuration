namespace Domore {
    using Configuration;

    class Program {
        static void Main() {
            new Sample { Configuration = new AppConfigContainer() }.Run();
        }
    }
}
