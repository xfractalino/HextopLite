namespace HextopLite.engine.graphics;

public interface IShader
{
    public void Load(string path);
    public void Attach();
    public void Detach();
    public IUniformBuffer CreateUniformBuffer(uint sizeBytes);
    public void Dispose();
}
