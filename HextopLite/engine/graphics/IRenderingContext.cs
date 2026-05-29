namespace HextopLite.engine.graphics;

public interface IRenderingContext
{
    public IRenderer Renderer { get; }
    internal Scene? Scene { get; set; }
    
    public uint Width { get; }
    public uint Height { get; }

    internal void Initialize(RendererType rendererType);
    internal void PreRender();
    internal void Render(double delta);
    internal bool DebugCheck();
    internal bool IsValid();
    internal void Dispose();
}