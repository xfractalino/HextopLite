using System.Runtime.InteropServices;
using Windows.UI.Composition;
using HextopLite.interop;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace HextopLite.engine;

/// <summary>
///   Main class responsible for the whole rendering lifecycle. It creates, employs and destroys graphics objects.
/// </summary>
public class Renderer
{
    private ID3D11Device _device = null!;
    private ID3D11DeviceContext _context = null!;
    private ID3D11RasterizerState _rasterizerState = null!;

    private Compositor _compositor = null!;
    private Windows.UI.Composition.Desktop.DesktopWindowTarget _target = null!;
    private CompositionDrawingSurface _surface = null!;
    private ICompositionDrawingSurfaceInterop _surfaceInterop = null!;
    
    private volatile int _running;

    private HextopWindow _hextopWindow = null!;
    private ShaderContext _shaderContext = null!;

    private uint _width, _height;
    
    private ManualResetEventSlim _cleanedUpGate = new (false);

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

        var renderer = new Thread(RunWithChecks);
        renderer.SetApartmentState(ApartmentState.STA);
        renderer.Start();
    }

    public void WaitUntilTermination()
    {
        _cleanedUpGate.Wait();
    }

    private void Init()
    {
        _hextopWindow = new HextopWindow();
        
        _width = _hextopWindow.Width;
        _height = _hextopWindow.Height;

        // Creates a D3D device
        D3D11.D3D11CreateDevice(
            null,
            DriverType.Hardware,
#if DEBUG
            DeviceCreationFlags.Debug |
#endif
            DeviceCreationFlags.BgraSupport, // This flag is required by the composition pipeline.
            [FeatureLevel.Level_11_0],
            out _device,
            out _,
            out _context
        );

        var dxgiDevice = _device.QueryInterface<IDXGIDevice>();

        // Creates a compositor for the Window. Windows requires this because DWM refuses to allow us to render using a
        // custom swapchain.
        _compositor = new Compositor();
        var interop = WinRT.CastExtensions.As<ICompositorDesktopInterop>(_compositor);
        interop.CreateDesktopWindowTarget(_hextopWindow.Hwnd, false, out var ptr);
        _target = Windows.UI.Composition.Desktop.DesktopWindowTarget.FromAbi(ptr);

        var visual = _compositor.CreateSpriteVisual();
        visual.Size = new System.Numerics.Vector2(_width, _height);
        _target.Root = visual;
        
        var compositorInterop = WinRT.CastExtensions.As<ICompositorInterop>(_compositor);
        compositorInterop.CreateGraphicsDevice(dxgiDevice.NativePointer, out var graphicsDevicePtr);
        var graphicsDevice = WinRT.MarshalInterface<CompositionGraphicsDevice>
            .FromAbi(graphicsDevicePtr);

        // We create a compatible surface to bridge Windows' composition to DirectX
        _surface = graphicsDevice.CreateDrawingSurface(
            new Windows.Foundation.Size(_width, _height),
            Windows.Graphics.DirectX.DirectXPixelFormat.R8G8B8A8UIntNormalized,
            Windows.Graphics.DirectX.DirectXAlphaMode.Premultiplied);

        _surfaceInterop = WinRT.CastExtensions.As<ICompositionDrawingSurfaceInterop>(_surface);

        var brush = _compositor.CreateSurfaceBrush(_surface);
        visual.Brush = brush;

        // Initialises a shader context with the default shader.
        _shaderContext = new ShaderContext(_device, _context);
        _shaderContext.LoadShader(Path.Combine(AppContext.BaseDirectory, "shaders", "default.hlsl"));
        
        var rasterizerDesc = new RasterizerDescription(CullMode.None, FillMode.Solid);
        _rasterizerState = _device.CreateRasterizerState(rasterizerDesc);
    }

    private void RunWithChecks()
    {
        try
        {
            Run();
        }
        catch (COMException exception)
        {
            Console.WriteLine(exception.ToString());
#if DEBUG
            CheckGpuState();
#endif
            Stop();
            Dispose();
        }
        finally
        {
            _cleanedUpGate.Set();
        }
    }

    private void Run()
    {
        Init();
        
        var textureGuid = typeof(ID3D11Texture2D).GUID;
        
        while (Interlocked.CompareExchange(ref _running, 1, 0) != 0 &&
               User32.IsWindow(_hextopWindow.Hwnd))
        {
            PeekWin32Messages();
#if DEBUG
            CheckGpuState();
#endif
            
            _surfaceInterop.BeginDraw(IntPtr.Zero, ref textureGuid, out var texturePtr, out var offset);
            
            _context.RSSetState(_rasterizerState);
    
            var texture = new ID3D11Texture2D(texturePtr);
            var rtv = _device.CreateRenderTargetView(texture);
    
            _context.OMSetRenderTargets(rtv);
            _context.RSSetViewport(offset.X, offset.Y, _width, _height);
            _context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
            _shaderContext.AttachCurrentShader();

            _context.ClearRenderTargetView(rtv, new Color4(1, 0, 0, 1));
            _context.Draw(3, 0);
    
            rtv.Dispose();
            texture.Dispose();
    
            _surfaceInterop.EndDraw();
        }
        
        Console.Write("Out of the render loop. ");

        Dispose();
    }
    
    private void Dispose()
    {
        Console.WriteLine("Now cleaning up.");

        // D3D resources
        _shaderContext.Dispose();
        _context.Dispose();
        _device.Dispose();

        // Composition
        _surfaceInterop = null!;
        _surface.Dispose();
        _target.Dispose();
        _compositor.Dispose();

        // Window
        if (User32.IsWindow(_hextopWindow.Hwnd))
            User32.DestroyWindow(_hextopWindow.Hwnd);

        // Restore Progman
        var progman = ProgmanSupervisor.Instance.ProgmanHwnd;
        User32.SendMessage(progman, 0x052C, 0xD, 0x0);
    }

    private static void PeekWin32Messages()
    {
        while (User32.PeekMessage(out var msg, 0, 0, 0, 1))
        {
            User32.TranslateMessage(ref msg);
            User32.DispatchMessage(ref msg);
        }
    }

#if DEBUG
    private void CheckGpuState()
    {
        var deviceRemovedReason = _device.DeviceRemovedReason;

        if (deviceRemovedReason.Failure)
        {
            Console.WriteLine(deviceRemovedReason.Description);
        }
    }
#endif
}