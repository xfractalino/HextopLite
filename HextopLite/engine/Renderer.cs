using System.Runtime.InteropServices;
using HextopLite.interop;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace HextopLite.engine;

public class Renderer
{
    private ID3D11Device _device = null!;
    private ID3D11DeviceContext _context = null!;
    private ID3D11RenderTargetView _renderTargetView = null!;
    private IDXGISwapChain _swapChain = null!;
    private IDXGISurface1 _dxgiSurface = null!;

    private Windows.UI.Composition.Compositor _compositor;
    private Windows.UI.Composition.Desktop.DesktopWindowTarget _target;

    private int _width, _height;
    private nint _parentHwnd;
    private nint _hextopHwnd;

    private nint _dqController;
    
    private volatile int _running;

    InteropCommons.WndProc _wndProc = User32.DefWindowProc;

    public static Renderer Instance
    {
        get
        {
            field ??= new Renderer();
            
            return field;
        }
    }

    public void Stop()
    {
        Interlocked.Exchange(ref _running, 0);
    }

    public void Start()
    {
        Interlocked.Exchange(ref _running, 1);

        var renderer = new Thread(Run);
        renderer.SetApartmentState(ApartmentState.STA);
        renderer.Start();
        renderer.Join();
    }

    private void Init()
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
            lpszClassName = "HextopWindow",
            hInstance = User32.GetModuleHandle(null)
        };

        User32.RegisterClassEx(ref wndClass);

        _hextopHwnd = User32.CreateWindowEx(
            User32.WS_EX_NOREDIRECTIONBITMAP,
            "HextopWindow", null!,
            User32.WS_VISIBLE | User32.WS_POPUP,
            0, 0, _width, _height,
            0, 0, 0, 0
        );
        
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

    private void Run()
    {
        Init();
        
        while (Interlocked.CompareExchange(ref _running, 1, 0) != 0 && User32.IsWindow(_hextopHwnd))
        {
            while (User32.PeekMessage(out InteropCommons.MSG msg, 0, 0, 0, 1))
            {
                if (msg.message == User32.WM_DESTROY)
                    Stop();

                User32.TranslateMessage(ref msg);
                User32.DispatchMessage(ref msg);
            }
        }
    }
}