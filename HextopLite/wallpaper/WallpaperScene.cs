using System.Numerics;
using System.Runtime.InteropServices;
using HextopLite.engine;
using HextopLite.engine.graphics;

namespace HextopLite.wallpaper;

public class WallpaperScene : Scene
{
    private IShader _defaultShader = null!;
    private IUniformBuffer _frameDataBuffer = null!;

    public override void OnInit()
    {
        if (RenderingContext == null)
            return;
        
        var renderer = RenderingContext.Renderer;

        _defaultShader = renderer.CreateShader(Path.Combine(AppContext.BaseDirectory, "shaders", "default.hlsl"));
        _frameDataBuffer = _defaultShader.CreateUniformBuffer((uint)Marshal.SizeOf<FrameData>());
    }
    
    public override void Render(double delta)
    {
        if (RenderingContext == null)
            return;
        
        var renderer = RenderingContext.Renderer;
        var metrics = RenderingEngine.Instance.Metrics;

        var frameData = new FrameData
        {
            Time = (float)metrics.TimeCount,
            DeltaTime = (float)delta,
            Resolution = new Vector2(RenderingContext.Width, RenderingContext.Height),
        };

        _defaultShader.Attach();
        _frameDataBuffer.Update(frameData);
        _frameDataBuffer.Bind(0);
        renderer.Draw(3, 0);
        _defaultShader.Detach();
    }

    public override void OnExit()
    {
        if (RenderingContext == null)
            return;
        
        _defaultShader.Detach();
        _defaultShader.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FrameData
    {
        public float Time;          // 4 bytes
        public float DeltaTime;     // 4 bytes
        public Vector2 Resolution;  // 8 bytes
    }
}