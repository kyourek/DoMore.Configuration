namespace Domore.Conf {
    public interface IConfKey : IConfToken {
        IConfCollection<IConfKeyPart> Parts { get; }
        IConfKey Skip();
    }
}
