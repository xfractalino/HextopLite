using HextopLite.engine.graphics;
using HextopLite.interop;
using HextopLite.wallpaper;
using Vortice.Mathematics;

namespace HextopLite.engine.windows;

/// <summary>
///   This represents a wallpaper rendering context on Windows.
/// </summary>
public class WindowsRenderingContext : IRenderingContext
{
    public IRenderer Renderer { get; private set; } = null!;

    public Scene? Scene
    {
        get;
        set
        {
            value?.RenderingContext = this;
            value?.OnInit();
            field = value;
        }
    }

    private HextopWindow _hextopWindow = null!;
    private WindowsCompositor _compositor = null!;

    private uint _width, _height;

    public uint Width => _width;
    public uint Height => _height;

    public void Initialize(RendererType rendererType)
    {
        _hextopWindow = new HextopWindow();
        
        _width = _hextopWindow.Width;
        _height = _hextopWindow.Height;

        // On Windows, we only give D3D support for now.
        // D3D is intrinsically bound to Windows' composition pipeline, so using other APIs with Windows.UI.Compositor
        // would be difficult and inefficient.
        // This switch is therefore kept here just in case we want to add web support in future which requires a
        // different renderer (probably still backed by D3D) or eventually D3D12.
        Renderer = rendererType switch
        {
            RendererType.D3D11 => new D3D11Renderer(),
            _ => throw new Exception($"Unsupported renderer on this platform: {rendererType}")
        };
        
        Renderer.Initialize();

        var renderer = (D3D11Renderer)Renderer;
        _compositor = new WindowsCompositor(_hextopWindow, renderer.DxgiDevice);
        _compositor.Initialize();

        renderer.Compositor = _compositor;
        
        renderer.SetRasterizerDescription(IRenderer.CullMode.None, IRenderer.FillMode.Solid);

        Scene = new WallpaperScene();
    }

    public void PreRender()
    {
        PeekWin32Messages();
    }

    public void Render(double delta)
    {
        Renderer.BeginDraw();
        Renderer.SetViewport(0, 0, _width, _height);
        Renderer.SetTopology(IRenderer.Topology.TriangleList);
        Renderer.Clear(new Color4(1, 0, 0));
        
        Scene?.Render(delta);
        
        Renderer.EndDraw();
    }

    public bool IsValid()
    {
        return User32.IsWindow(_hextopWindow.Hwnd) && DebugCheck();
    }

    public void Dispose()
    {
        Renderer.Dispose();

        _compositor.Dispose();

        if (User32.IsWindow(_hextopWindow.Hwnd))
            User32.DestroyWindow(_hextopWindow.Hwnd);
        
        var progman = ProgmanSupervisor.Instance.ProgmanHwnd;
        User32.SendMessage(progman, 0x052C, 0xD, 0x0);
    }

    public bool DebugCheck()
    {
#if DEBUG
        if (Renderer is D3D11Renderer d3d11Renderer)
            return d3d11Renderer.CheckGpuState();
#endif
        return true;
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