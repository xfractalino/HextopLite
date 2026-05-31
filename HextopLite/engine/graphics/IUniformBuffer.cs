namespace HextopLite.engine.graphics;

public interface IUniformBuffer : IDisposable
{
    void Update<T>(in T data) where T : unmanaged;
    void Bind(uint slot);
}