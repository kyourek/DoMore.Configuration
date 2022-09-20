namespace Domore.Conf {
    public interface IConfPair : IConfToken {
        IConfKey Key { get; }
        IConfValue Value { get; }
    }
}
