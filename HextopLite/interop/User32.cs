using System.Runtime.InteropServices;

namespace HextopLite.interop;

public static class User32
{
    // ReSharper disable InconsistentNaming
    public const uint WM_DESTROY = 0x0002;
    public const uint WM_CLOSE = 0x0010;

    public const uint WS_POPUP = 0x80000000;
    public const uint WS_CHILD = 0x40000000;
    public const uint WS_VISIBLE = 0x10000000;
    public const uint WS_CLIPSIBLINGS = 0x04000000;
    public const int WS_EX_TRANSPARENT = 0x00000020;
    public const int WS_EX_TOOLWINDOW = 0x00000080;
    public const int WS_EX_APPWINDOW = 0x00040000;
    public const int WS_EX_NOREDIRECTIONBITMAP = 0x00200000;

    public const int GWL_STYLE = -16;
    public const int GWL_EXSTYLE = -20;

    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_NOACTIVATE = 0x0010;
    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOMOVE = 0x0002;
    // ReSharper enable InconsistentNaming
    
    [DllImport("user32.dll")]
    public static extern nint FindWindow(string cls, string win);

    [DllImport("user32.dll")]
    public static extern nint FindWindowEx(nint parent, nint after, string cls, string win);

    [DllImport("user32.dll")]
    public static extern nint SendMessage(nint hWnd, uint msg, nint w, nint l);

    [DllImport("user32.dll")]
    public static extern nint SetParent(nint child, nint newParent);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(InteropCommons.EnumWindowsProc proc, nint lParam);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(nint hWnd);
    
    [DllImport("user32.dll")]
    public static extern bool GetClientRect(nint hWnd, out InteropCommons.RECT rect);
    
    [DllImport("user32.dll")]
    public static extern bool PeekMessage(out InteropCommons.MSG msg, nint hWnd, uint min, uint max, uint remove);

    [DllImport("user32.dll")]
    public static extern bool TranslateMessage(ref InteropCommons.MSG msg);

    [DllImport("user32.dll")]
    public static extern nint DispatchMessage(ref InteropCommons.MSG msg);

    [DllImport("user32.dll")]
    public static extern nint GetDC(nint hWnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(nint hWnd, nint hDC);
    
    [DllImport("user32.dll")]
    public static extern nint CreateWindowEx(
        int exStyle, string className, string windowName,
        uint style, int x, int y, int width, int height,
        nint parent, nint menu, nint instance, nint param);
    
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

    [DllImport("user32.dll")]
    public static extern nint DefWindowProc(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern nint GetModuleHandle(string? lpModuleName);

    [DllImport("user32.dll")]
    public static extern bool IsWindow(nint hWnd);
    
    [DllImport("user32.dll")]
    public static extern int GetWindowLong(nint hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern long GetWindowLongPtr(nint hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy,
        uint uFlags);

    [DllImport("user32.dll")]
    public static extern bool DestroyWindow(nint hWnd);
    
    [DllImport("user32.dll")]
    public static extern int FillRect(nint hdc, ref InteropCommons.RECT rect, nint hbr);
    
    [DllImport("user32.dll")]
    public static extern void PostQuitMessage(int nExitCode);
    
    // ReSharper disable InconsistentNaming
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WNDCLASSEX
    {
        public uint cbSize;
        public uint style;
        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
        public nint hIconSm;
    }
    // ReSharper enable InconsistentNaming
}