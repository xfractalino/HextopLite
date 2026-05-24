using System.Runtime.InteropServices;

namespace HextopLite.interop;

[ComImport]
[Guid("29E691FA-4567-4DCA-B319-D0F207EB6807")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface ICompositorDesktopInterop
{
    void CreateDesktopWindowTarget(IntPtr hwndTarget, bool isTopmost, out IntPtr result);
}