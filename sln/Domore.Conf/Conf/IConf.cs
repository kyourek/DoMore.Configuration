namespace Domore.Conf {
    public interface IConf {
        T Configure<T>(T target, string key = null);
    }
}
