using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("F5BE22A8-89C0-4FD6-924D-156DE83B54BE")]
    [ComVisible(true)]
#if NETCOREAPP
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
#else
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
#endif
    public interface IConfigurationBlockItem {
        [DispId(1)]
        string OriginalKey { get; }

        [DispId(2)]
        string NormalizedKey { get; }

        [DispId(3)]
        string OriginalValue { get; }

        [DispId(4)]
        string StringValue { get; }

        [DispId(5)]
        int IntegerValue { get; }

        [DispId(6)]
        double NumberValue { get; }

        [DispId(7)]
        bool BooleanValue { get; }
    }
}
