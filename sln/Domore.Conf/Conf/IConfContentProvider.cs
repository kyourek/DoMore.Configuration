namespace Domore.Conf {
    public interface IConfContentProvider {
        IConfContent GetConfContent(object source);
    }
}
