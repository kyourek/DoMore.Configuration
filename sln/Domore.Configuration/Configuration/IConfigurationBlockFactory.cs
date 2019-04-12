using System.Runtime.InteropServices;

namespace Domore.Configuration {
    [Guid("C19731E5-3817-43FE-9FC1-20351655171D")]
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IConfigurationBlockFactory {
        [DispId(1)]
        IConfigurationBlock CreateConfigurationBlock(string configuration);
    }
}
