using HextopLite.interop;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace HextopLite.engine;

public class Renderer
{
    private ID3D11Device _device = null!;
    private ID3D11DeviceContext _context = null!;
    private ID3D11RenderTargetView _renderTargetView = null!;
    private IDXGISwapChain _swapChain = null!;
    private IDXGISurface1 _dxgiSurface = null!;
    
    private volatile int _running;

    private HextopWindow _hextopWindow = null!;

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
        _hextopWindow = new HextopWindow();
    }

    private void Run()
    {
        Init();
        
        while (Interlocked.CompareExchange(ref _running, 1, 0) != 0 &&
               User32.IsWindow(_hextopWindow.Hwnd))
        {
            PeekWin32Messages();
        }

        if (User32.IsWindow(_hextopWindow.Hwnd))
        {
            // Our window is still alive, so we destroy it.
            // This happens when the program is closed externally, e.g. Ctrl+C in console.
            User32.SendMessage(_hextopWindow.Hwnd, User32.WM_DESTROY, 0, 0);
            PeekWin32Messages();
        }
    }

    private static void PeekWin32Messages()
    {
        while (User32.PeekMessage(out var msg, 0, 0, 0, 1))
        {
            User32.TranslateMessage(ref msg);
            User32.DispatchMessage(ref msg);
        }
    }
}