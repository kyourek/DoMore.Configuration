using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("DE0F8A37-ABBB-426A-A2B0-6C4AACFEBD34")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConfigurationDefault {
        [DispId(1)]
        IConfigurationContainer Container { get; }
    }
}
