namespace Domore.Conf.Future {
    public interface IConf {
        T Configure<T>(T target, string key = null);
    }
}
