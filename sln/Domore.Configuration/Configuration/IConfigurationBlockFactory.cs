using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("C19731E5-3817-43FE-9FC1-20351655171D")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConfigurationBlockFactory {
        [DispId(1)]
        IConfigurationBlock CreateConfigurationBlock(object content, IConfigurationContentsFactory contentsFactory);
    }
}
