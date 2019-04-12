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
        string String(object key);

        [DispId(6)]
        string StringOrDefault(object key, string def);

        [DispId(7)]
        int Integer(object key);

        [DispId(8)]
        int IntegerOrDefault(object key, int def);

        [DispId(9)]
        double Number(object key);

        [DispId(10)]
        double NumberOrDefault(object key, double def);

        [DispId(11)]
        bool Boolean(object key);

        [DispId(12)]
        bool BooleanOrDefault(object key, bool def);

        [DispId(13)]
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
