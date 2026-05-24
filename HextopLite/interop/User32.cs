using System.Runtime.InteropServices;

namespace HextopLite.interop;

public static class User32
{
    // ReSharper disable InconsistentNaming
    public const uint WM_DESTROY = 0x0002;

    public const uint WS_POPUP = 0x80000000;
    public const uint WS_CHILD = 0x40000000;
    public const uint WS_VISIBLE = 0x10000000;
    public const uint WS_CLIPSIBLINGS = 0x04000000;

    public const int GWL_STYLE = -16;

    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_NOACTIVATE = 0x0010;
    // ReSharper enable InconsistentNaming
    
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string cls, string win);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr after, string cls, string win);

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr w, IntPtr l);

    [DllImport("user32.dll")]
    public static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(InteropCommons.EnumWindowsProc proc, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);
    
    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, out InteropCommons.RECT rect);
    
    [DllImport("user32.dll")]
    public static extern bool PeekMessage(out InteropCommons.MSG msg, IntPtr hWnd, uint min, uint max, uint remove);

    [DllImport("user32.dll")]
    public static extern bool TranslateMessage(ref InteropCommons.MSG msg);

    [DllImport("user32.dll")]
    public static extern IntPtr DispatchMessage(ref InteropCommons.MSG msg);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    
    [DllImport("user32.dll")]
    public static extern IntPtr CreateWindowEx(
        int exStyle, string className, string windowName,
        uint style, int x, int y, int width, int height,
        IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);
    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

    [DllImport("user32.dll")]
    public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("user32.dll")]
    public static extern bool IsWindow(IntPtr hWnd);
    
    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
        uint uFlags);
    
    [DllImport("user32.dll")]
    public static extern int FillRect(IntPtr hdc, ref InteropCommons.RECT rect, IntPtr hbr);
    
    // ReSharper disable InconsistentNaming
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WNDCLASSEX
    {
        public uint cbSize;
        public uint style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }
    // ReSharper enable InconsistentNaming
}