using System.Runtime.InteropServices;
using HextopLite.engine.graphics;
using Vortice.Direct3D11;

namespace HextopLite.engine.windows;

public class HlslUniformBuffer : IUniformBuffer
{
    // ReSharper disable once NotAccessedField.Local
    private readonly ID3D11Device _device;
    private readonly ID3D11DeviceContext _context;
    private readonly ID3D11Buffer _buffer;

    private readonly uint _sizeBytes;
    
    internal HlslUniformBuffer(ID3D11Device device, ID3D11DeviceContext context, uint sizeBytes)
    {
        _device = device;
        _context = context;
        
        _sizeBytes = sizeBytes;
        
        _buffer = device.CreateBuffer(new BufferDescription
        {
            ByteWidth = sizeBytes,
            Usage = ResourceUsage.Dynamic,
            BindFlags = BindFlags.ConstantBuffer,
            CPUAccessFlags = CpuAccessFlags.Write
        });
    }
    
    public void Update<T>(in T data) where T : unmanaged
    {
        // ReSharper disable once RedundantArgumentDefaultValue
        var mapped = _context.Map(_buffer, 0, MapMode.WriteDiscard, MapFlags.None);
        MemoryMarshal.Write(mapped.AsSpan((int)_sizeBytes), in data);
        _context.Unmap(_buffer, 0);
    }

    public void Bind(uint slot)
    {
        _context.VSSetConstantBuffer(slot, _buffer);
        _context.PSSetConstantBuffer(slot, _buffer);
    }

    public void Dispose()
    {
        _buffer.Dispose();
    }
}