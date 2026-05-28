namespace HextopLite.engine;

internal interface IRenderingContext
{
    internal IRenderer Renderer { get; }

    public void Initialize(RendererType rendererType);
    public void PreRender();
    public void Render();
    public bool DebugCheck();
    public bool IsValid();
    public void Dispose();
}