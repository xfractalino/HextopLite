using Windows.UI.Composition;
using HextopLite.interop;
using Vortice.DXGI;

namespace HextopLite.engine.windows;

public class WindowsCompositor(HextopWindow hextopWindow, IDXGIDevice dxgiDevice) : IRenderingResource
{
    internal Compositor Compositor = null!;
    internal Windows.UI.Composition.Desktop.DesktopWindowTarget Target = null!;
    internal CompositionDrawingSurface Surface = null!;
    internal ICompositionDrawingSurfaceInterop SurfaceInterop = null!;

    public void Initialize()
    {
        uint width = hextopWindow.Width;
        uint height = hextopWindow.Height;
        
        // Creates a compositor for the Window. Windows requires this because DWM refuses to allow us to render using a
        // custom swapchain.
        Compositor = new Compositor();
        var interop = WinRT.CastExtensions.As<ICompositorDesktopInterop>(Compositor);
        interop.CreateDesktopWindowTarget(hextopWindow.Hwnd, false, out var ptr);
        Target = Windows.UI.Composition.Desktop.DesktopWindowTarget.FromAbi(ptr);

        var visual = Compositor.CreateSpriteVisual();
        visual.Size = new System.Numerics.Vector2(width, height);
        Target.Root = visual;
        
        var compositorInterop = WinRT.CastExtensions.As<ICompositorInterop>(Compositor);
        compositorInterop.CreateGraphicsDevice(dxgiDevice.NativePointer, out var graphicsDevicePtr);
        var graphicsDevice = WinRT.MarshalInterface<CompositionGraphicsDevice>.FromAbi(graphicsDevicePtr);

        // We create a compatible surface to bridge Windows' composition to DirectX
        Surface = graphicsDevice.CreateDrawingSurface(
            new Windows.Foundation.Size(width, height),
            Windows.Graphics.DirectX.DirectXPixelFormat.R8G8B8A8UIntNormalized,
            Windows.Graphics.DirectX.DirectXAlphaMode.Premultiplied);
        
        SurfaceInterop = WinRT.CastExtensions.As<ICompositionDrawingSurfaceInterop>(Surface);

        var brush = Compositor.CreateSurfaceBrush(Surface);
        visual.Brush = brush;
    }

    public void Dispose()
    {
        SurfaceInterop = null!;
        Surface.Dispose();
        Target.Dispose();
        Compositor.Dispose();
    }
}