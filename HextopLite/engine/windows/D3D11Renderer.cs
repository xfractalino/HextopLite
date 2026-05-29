using System.Runtime.CompilerServices;
using HextopLite.interop;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using Color = System.Drawing.Color;

namespace HextopLite.engine.windows;

public class D3D11Renderer : IRenderer
{
    private ID3D11Device _device = null!;
    private ID3D11DeviceContext _context = null!;
    private IDXGIDevice _dxgiDevice = null!;

    internal IDXGIDevice DxgiDevice => _dxgiDevice;

    internal WindowsCompositor Compositor { get; set; } = null!;

    private nint _surfaceTextureHandle;
    private InteropCommons.POINT _surfaceTextureOffset;

    private ID3D11Texture2D _renderTexture = null!;
    private ID3D11RenderTargetView _renderTargetView = null!;
    private ID3D11RasterizerState _rasterizerState = null!;

    private List<IShader> _shadersRefs = [];

    public void Initialize()
    {
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

        _dxgiDevice = _device.QueryInterface<IDXGIDevice>();
    }

    public void Dispose()
    {
        foreach (var shader in _shadersRefs)
            shader.Dispose();
        
        _shadersRefs.Clear();
        
        _context.Dispose();
        _device.Dispose();
    }

    public IShader CreateShader(string path)
    {
        var shader = new HlslShader(_device, _context);
        shader.Load(path);
        
        _shadersRefs.Add(shader);

        return shader;
    }

    public void SetRasterizerDescription(IRenderer.CullMode cullMode, IRenderer.FillMode fillMode)
    {
        var cullModeD3D = cullMode switch
        {
            IRenderer.CullMode.Back => CullMode.Back,
            IRenderer.CullMode.Front => CullMode.Front,
            IRenderer.CullMode.None => CullMode.None,
            _ => throw new ArgumentOutOfRangeException(nameof(cullMode), cullMode, null)
        };

        var fillModeD3D = fillMode switch
        {
            IRenderer.FillMode.Solid => FillMode.Solid,
            IRenderer.FillMode.Wireframe => FillMode.Wireframe,
            _ => throw new ArgumentOutOfRangeException(nameof(fillMode), fillMode, null)
        };

        _rasterizerState = _device.CreateRasterizerState(new RasterizerDescription(cullModeD3D, fillModeD3D));
    }

    public void Draw(uint n, uint i)
    {
        _context.Draw(n, i);
    }

    public void SetTopology(IRenderer.Topology topology)
    {
        _context.IASetPrimitiveTopology(TopologyToD3DPrimitiveTopology(topology));
    }

    public void SetViewport(int x, int y, uint width, uint height)
    {
        _context.RSSetViewport(x + _surfaceTextureOffset.X, y + _surfaceTextureOffset.Y, width, height);
    }

    public void Clear(Color4 color)
    {
        _context.ClearRenderTargetView(_renderTargetView, new Color4(color.R, color.G, color.B,
            color.A));
    }

    public void BeginDraw()
    {
        var textureGuid = typeof(ID3D11Texture2D).GUID;
        Compositor.SurfaceInterop.BeginDraw(IntPtr.Zero, ref textureGuid, out _surfaceTextureHandle,
            out _surfaceTextureOffset);
        _context.RSSetState(_rasterizerState);
        
        _renderTexture = new ID3D11Texture2D(_surfaceTextureHandle);
        _renderTargetView = _device.CreateRenderTargetView(_renderTexture);
        
        _context.OMSetRenderTargets(_renderTargetView);
    }

    public void EndDraw()
    {
        Compositor.SurfaceInterop.EndDraw();
    
        _renderTargetView.Dispose();
        _renderTexture.Dispose();
    }

#if DEBUG
    public bool CheckGpuState()
    {
        var deviceRemovedReason = _device.DeviceRemovedReason;

        if (deviceRemovedReason.Failure)
        {
            Console.WriteLine(deviceRemovedReason.Description);

            return false;
        }

        return true;
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PrimitiveTopology TopologyToD3DPrimitiveTopology(IRenderer.Topology topology)
    {
        return (PrimitiveTopology)topology;
    }
}