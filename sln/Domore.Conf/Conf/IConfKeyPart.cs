namespace Domore.Conf {
    public interface IConfKeyPart : IConfToken {
        IConfCollection<IConfKeyIndex> Indices { get; }
    }
}
