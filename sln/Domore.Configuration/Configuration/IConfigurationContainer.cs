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
        IConfigurationBlockFactory BlockFactory { get; set; }

        [DispId(4)]
        IConfigurationBlock Block { get; }

        [DispId(5)]
        event EventHandler ContentsChanged;

        [DispId(6)]
        string Value(object key);

        [DispId(7)]
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
