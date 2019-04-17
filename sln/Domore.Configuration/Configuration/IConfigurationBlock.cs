using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("884A0522-59A1-4305-92A5-3F53FCFE4E52")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConfigurationBlock {
        [DispId(1)]
        object Content { get; }

        [DispId(2)]
        IConfigurationContentsFactory ContentsFactory { get; }

        [DispId(3)]
        int ItemCount { get; }

        [DispId(4)]
        bool ItemExists(object key);

        [DispId(5)]
        IConfigurationBlockItem Item(object key);

        [DispId(6)]
        object Configure(object obj, string key = null);

        [ComVisible(false)]
        T Configure<T>(T obj, string key = null);
    }
}
