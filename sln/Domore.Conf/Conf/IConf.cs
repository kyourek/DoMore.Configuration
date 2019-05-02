using System.Runtime.InteropServices;

namespace Domore.Conf {
    [Guid("DE0F8A37-ABBB-426A-A2B0-6C4AACFEBD34")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConf {
        [DispId(1)]
        IConfContainer Container { get; }
    }
}
