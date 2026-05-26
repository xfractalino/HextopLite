using System.Runtime.InteropServices;
using HextopLite.interop;

namespace HextopLite.engine;

public class HextopWindow
{
    private const string HextopClassName = "HextopWindow";
    
    private Windows.UI.Composition.Compositor _compositor = null!;
    private Windows.UI.Composition.Desktop.DesktopWindowTarget _target = null!;

    private int _width, _height;
    private nint _parentHwnd;
    private nint _hextopHwnd;

    private nint _dqController;

    private readonly InteropCommons.WndProc _wndProc = WndProc;

    public HextopWindow()
    {
        var progmanHwnd = ProgmanSupervisor.Instance.ProgmanHwnd;
        
        var progmanExStyle = User32.GetWindowLongPtr(progmanHwnd, User32.GWL_EXSTYLE);
        if ((progmanExStyle & User32.WS_EX_NOREDIRECTIONBITMAP) != 0)
        {
            _parentHwnd = progmanHwnd;
        }
        else
        {
            _parentHwnd = ProgmanSupervisor.Instance.WorkerWHwnd;
        }
        
        var options = new InteropCommons.DispatcherQueueOptions
        {
            dwSize = (uint)Marshal.SizeOf<InteropCommons.DispatcherQueueOptions>(),
            threadType = 2,    // DQTYPE_THREAD_CURRENT
            apartmentType = 0  // DQTAT_COM_NONE
        };
        Marshal.ThrowExceptionForHR(InteropCommons.CreateDispatcherQueueController(options, out _dqController));
        
        Console.WriteLine("Found target hwnd: {0:X}", _parentHwnd);
        
        User32.GetClientRect(_parentHwnd, out var rect);
        rect.ToExtents(out _width, out _height);
        
        var wndClass = new User32.WNDCLASSEX
        {
            cbSize = (uint)Marshal.SizeOf<User32.WNDCLASSEX>(),
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndProc),
            lpszClassName = HextopClassName,
            hInstance = User32.GetModuleHandle(null)
        };

        User32.RegisterClassEx(ref wndClass);

        _hextopHwnd = User32.CreateWindowEx(
            User32.WS_EX_NOREDIRECTIONBITMAP,
            HextopClassName, null!,
            User32.WS_VISIBLE | User32.WS_POPUP,
            0, 0, _width, _height,
            0, 0, 0, 0
        );
        if (_hextopHwnd == 0)
        {
            Console.WriteLine($"Failed to create window.");
            return;
        }
        
        _compositor = new Windows.UI.Composition.Compositor();
        var interop = WinRT.CastExtensions.As<ICompositorDesktopInterop>(_compositor);
        interop.CreateDesktopWindowTarget(_hextopHwnd, false, out var ptr);
        _target = Windows.UI.Composition.Desktop.DesktopWindowTarget.FromAbi(ptr);

        var visual = _compositor.CreateSpriteVisual();
        visual.Size = new System.Numerics.Vector2(_width, _height);
        visual.Brush = _compositor.CreateColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
        _target.Root = visual;
        
        int style = User32.GetWindowLong(_hextopHwnd, User32.GWL_STYLE);
        style |= (int)User32.WS_CHILD;
        style &= (int)~User32.WS_POPUP;
        User32.SetWindowLong(_hextopHwnd, User32.GWL_STYLE, style);
        User32.SetParent(_hextopHwnd, _parentHwnd);

        User32.SetWindowPos(_hextopHwnd, ProgmanSupervisor.Instance.ShellViewHwnd,
            0, 0, _width, _height, User32.SWP_NOACTIVATE);
    }

    public nint Hwnd => _hextopHwnd;

    private static nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        switch (msg)
        {
            case User32.WM_DESTROY:
                Console.WriteLine("Destroy message received. Stopping the renderer.");
                
                Renderer.Instance.Stop();
                User32.PostQuitMessage(0);

                return 0;
        }

        return User32.DefWindowProc(hwnd, msg, wParam, lParam);
    }
}