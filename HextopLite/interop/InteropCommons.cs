using System.Runtime.InteropServices;

namespace HextopLite.interop;

public static class InteropCommons
{
    // ReSharper disable InconsistentNaming
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

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
    public struct MSG { public IntPtr hWnd, wParam, lParam; public uint message, time; public POINT pt; }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT { public int x, y; }
    // ReSharper enable InconsistentNaming
}