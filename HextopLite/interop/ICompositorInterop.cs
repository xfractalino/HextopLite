using System.Runtime.InteropServices;

namespace HextopLite.interop;

[ComImport]
[Guid("25297D5C-3AD4-4C9C-B5CF-E36A38512330")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICompositorInterop
{
    void CreateCompositionSurfaceForHandle(nint swapChain, out nint surface);
    void CreateCompositionSurfaceForSwapChain(nint swapChain, out nint surface);
    void CreateGraphicsDevice(nint renderingDevice, out nint result);
}