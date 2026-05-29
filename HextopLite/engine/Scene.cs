using HextopLite.engine.graphics;

namespace HextopLite.engine;

public abstract class Scene
{
    public IRenderingContext? RenderingContext { get; set; }
    
    public abstract void Render(double delta);
    public virtual void OnInit() {}
    public virtual void OnExit() {}
}