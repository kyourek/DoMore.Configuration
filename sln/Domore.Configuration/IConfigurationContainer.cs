using System;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("A3C65EC6-4B02-4700-8B86-6723E09363BD")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConfigurationContainer {
        [DispId(1)]
        object Content { get; set; }

        [DispId(2)]
        IConfigurationContentsProvider ContentsProvider { get; set; }

        [DispId(3)]
        IConfigurationBlock Block { get; }

        [DispId(4)]
        event EventHandler ContentsChanged;
    }
}
