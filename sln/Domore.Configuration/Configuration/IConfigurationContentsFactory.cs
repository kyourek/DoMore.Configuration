using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("315A04EF-483C-4665-80AE-F9EC4FDBD2D2")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConfigurationContentsFactory {
        [DispId(1)]
        IEnumerable<KeyValuePair<string, string>> CreateConfigurationContents(object content);
    }
}
