using System.Runtime.InteropServices;

namespace HextopLite.interop;

public static class DwmInterop
{
    [DllImport("dcomp.dll")]
    public static extern int DCompositionWaitForCompositorClock(
        uint count,
        nint[]? handles,
        uint timeoutInMs);
    
    [DllImport("dwmapi.dll")]
    public static extern int DwmFlush();
    
    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(nint hwnd, int attr, out int value, int size);
}