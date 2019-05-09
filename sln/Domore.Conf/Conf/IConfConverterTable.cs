using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Domore.Conf {
    [Guid("1E49BD76-E259-4AB4-A87D-9E14B962A8EC")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConfConverterTable {
        [DispId(1)]
        TypeConverter this[Type type] { set; }
    }
}
