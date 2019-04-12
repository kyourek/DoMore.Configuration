using System;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("22BDB2A4-ADD8-4EB1-A5B5-EA155BB49220")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IAppConfigContainer : IConfigurationContainer {
        [DispId(1)]
        string ExePath { get; set; }
    }
}
