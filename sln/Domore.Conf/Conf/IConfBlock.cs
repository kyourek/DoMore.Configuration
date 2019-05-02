using System.Runtime.InteropServices;

namespace Domore.Conf {
    [Guid("884A0522-59A1-4305-92A5-3F53FCFE4E52")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConfBlock {
        [DispId(1)]
        int ItemCount();

        [DispId(2)]
        bool ItemExists(object key);

        [DispId(3)]
        IConfBlockItem Item(object key);

        [DispId(4)]
        object Configure(object obj, string key = null);

        [ComVisible(false)]
        T Configure<T>(T obj, string key = null);

        [ComVisible(false)]
        bool ItemExists(object key, out IConfBlockItem item);
    }
}
