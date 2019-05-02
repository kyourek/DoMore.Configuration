using System;
using System.Runtime.InteropServices;

namespace Domore.Conf {
    [Guid("A3C65EC6-4B02-4700-8B86-6723E09363BD")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConfContainer {
        [DispId(1)]
        object Content { get; set; }

        [DispId(2)]
        IConfContentsProvider ContentsProvider { get; set; }

        [DispId(3)]
        IConfBlock Block { get; }

        [DispId(4)]
        event EventHandler ContentsChanged;
    }
}
