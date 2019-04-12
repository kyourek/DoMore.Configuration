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
        string Content { get; set; }

        [DispId(2)]
        IConfigurationBlockFactory BlockFactory { get; set; }

        [DispId(3)]
        IConfigurationBlock Block { get; }

        [DispId(4)]
        event EventHandler Changed;

        [DispId(5)]
        string Value(object key);

        [DispId(6)]
        object Configure(object obj, string key = null);

        [ComVisible(false)]
        T Value<T>(object key);

        [ComVisible(false)]
        T Value<T>(object key, T def);

        [ComVisible(false)]
        T Value<T>(object key, out T value);

        [ComVisible(false)]
        T Value<T>(object key, out T value, T def);

        [ComVisible(false)]
        T Configure<T>(T obj, string key = null);
    }
}
