namespace Domore.Conf.Future {
    internal interface IConfPair : IConfToken {
        IConfKey Key { get; }
        IConfValue Value { get; }
    }
}
