using System.Runtime.InteropServices;

namespace HextopLite.interop;

public static class Gdi32
{
    [DllImport("gdi32.dll")]
    public static extern bool BitBlt(IntPtr hdc, int x, int y, int cx, int cy,
        IntPtr hdcSrc, int x1, int y1, uint rop);
    
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateSolidBrush(uint color);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);
}