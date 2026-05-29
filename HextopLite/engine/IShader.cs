namespace HextopLite.engine;

public interface IShader
{
    public void Load(string path);
    public void Attach();
    public void Detach();
    public void Dispose();
}
