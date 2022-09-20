namespace Domore.Conf.Future {
    internal interface IConfKey : IConfToken {
        IConfCollection<IConfKeyPart> Parts { get; }
        IConfKey Skip();
    }
}
