using System.Runtime.InteropServices;

namespace HextopLite.interop;

public static class InteropCommons
{
    // ReSharper disable InconsistentNaming
    public delegate bool EnumWindowsProc(nint hWnd, nint lParam);
    public delegate nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;

        public void ToExtents(out int width, out int height)
        {
            width = Right - Left;
            height = Bottom - Top;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG { public nint hWnd, wParam, lParam; public uint message, time; public POINT pt; }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT { public int X, Y; }
    // ReSharper enable InconsistentNaming
    
    [StructLayout(LayoutKind.Sequential)]
    public struct DispatcherQueueOptions
    {
        public uint dwSize;
        public int threadType;
        public int apartmentType;
    }
    
    [DllImport("CoreMessaging.dll")]
    public static extern int CreateDispatcherQueueController(
        DispatcherQueueOptions options,
        out nint dispatcherQueueController);
}