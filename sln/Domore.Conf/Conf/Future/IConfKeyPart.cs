namespace Domore.Conf.Future {
    internal interface IConfKeyPart : IConfToken {
        IConfCollection<IConfKeyIndex> Indices { get; }
    }
}
