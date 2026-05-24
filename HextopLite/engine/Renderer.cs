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

    private int _width, _height;
    private IntPtr _hextopHwnd;
    
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
        renderer.Start();
        renderer.Join();
    }

    private void Init()
    {
        var workerWHwnd = ProgmanSupervisor.Instance.WorkerWHwnd;
        
        User32.GetClientRect(workerWHwnd, out var rect);
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
            0, "HextopWindow", null!,
            User32.WS_VISIBLE | User32.WS_POPUP,
            0, 0, _width, _height,
            IntPtr.Zero,
            IntPtr.Zero, IntPtr.Zero, IntPtr.Zero
        );

        var swapChainDesc = new SwapChainDescription
        {
            BufferCount = 2,
            BufferDescription = new ModeDescription((uint)_width, (uint)_height, Format.R8G8B8A8_UNorm),
            BufferUsage = Usage.RenderTargetOutput,
            OutputWindow = _hextopHwnd,
            SampleDescription = new SampleDescription(1, 0),
            SwapEffect = SwapEffect.Discard,
            Windowed = true,
        };
        
        D3D11.D3D11CreateDeviceAndSwapChain(
            null,
            DriverType.Hardware,
            DeviceCreationFlags.None,
            new[] { FeatureLevel.Level_11_0 },
            swapChainDesc,
            out _swapChain!,
            out _device!,
            out _,
            out _context!
        );
        
        var textureDesc = new Texture2DDescription
        {
            Width = (uint)_width,
            Height = (uint)_height,
            MipLevels = 1,
            ArraySize = 1,
            Format = Format.B8G8R8A8_UNorm,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.RenderTarget,
            MiscFlags = ResourceOptionFlags.GdiCompatible
        };
        
        var renderTexture = _device.CreateTexture2D(textureDesc);
        _renderTargetView = _device.CreateRenderTargetView(renderTexture);
        _dxgiSurface = renderTexture.QueryInterface<IDXGISurface1>();
        
        User32.SetParent(_hextopHwnd, workerWHwnd);
        
        var style = User32.GetWindowLong(_hextopHwnd, User32.GWL_STYLE);
        User32.SetWindowLong(_hextopHwnd, User32.GWL_STYLE,
            (int)((style | User32.WS_CHILD) & ~User32.WS_POPUP));
        User32.SetWindowPos(_hextopHwnd, 1, 0, 0, _width, _height,
            User32.SWP_NOZORDER);
    }

    private void Run()
    {
        Init();
        
        while (Interlocked.CompareExchange(ref _running, 1, 0) != 0 && User32.IsWindow(_hextopHwnd))
        {
            while (User32.PeekMessage(out InteropCommons.MSG msg, _hextopHwnd, 0, 0, 1))
            {
                if (msg.message == User32.WM_DESTROY)
                    Stop();

                User32.TranslateMessage(ref msg);
                User32.DispatchMessage(ref msg);
            }
            
            _context.ClearRenderTargetView(_renderTargetView, new Color4(0, 0, 0));
            _swapChain.Present(1, PresentFlags.None);
        }
    }
}