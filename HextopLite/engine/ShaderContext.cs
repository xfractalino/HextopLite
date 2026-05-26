using SharpGen.Runtime;
using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;

namespace HextopLite.engine;

internal class ShaderContext
{
    private readonly ID3D11Device _device;
    private readonly ID3D11DeviceContext _context;

    private ID3D11VertexShader? _vertexShaderHandle;
    private ID3D11PixelShader? _fragmentShaderHandle;

    internal ShaderContext(ID3D11Device device, ID3D11DeviceContext context)
    {
        _device = device;
        _context = context;
    }

    internal void LoadShader(string hlslPath)
    {
        var result = Compiler.CompileFromFile(hlslPath, "VSMain", "vs_5_0", out var vsBlob, out var errorBlob);

        if (!ShaderCompileCheck(result, errorBlob))
            return;
        
        result = Compiler.CompileFromFile(hlslPath, "PSMain", "ps_5_0", out var psBlob, out errorBlob);

        if (!ShaderCompileCheck(result, errorBlob))
            return;

        _vertexShaderHandle = _device.CreateVertexShader(vsBlob.AsBytes());
        _fragmentShaderHandle = _device.CreatePixelShader(psBlob.AsBytes());
        return;

        bool ShaderCompileCheck(Result checkResult, Blob error)
        {
            if (!checkResult.Failure)
                return true;

            var errorMessage = System.Text.Encoding.ASCII.GetString(error.AsBytes());
            
            Console.WriteLine("Error compiling shader \"{0}\": {1}", hlslPath, errorMessage);
            return false;

        }
    }

    internal void AttachCurrentShader()
    {
        if (_vertexShaderHandle == null || _fragmentShaderHandle == null || 
            _vertexShaderHandle?.NativePointer == 0 || _fragmentShaderHandle?.NativePointer == 0)
        {
#if DEBUG
            Console.WriteLine("One of the shader pointers is null. Skipping attachment.");
#endif
            return;
        }
        
        _context.VSSetShader(_vertexShaderHandle);
        _context.PSSetShader(_fragmentShaderHandle);
    }

    internal void Dispose()
    {
        _vertexShaderHandle?.Dispose();
        _fragmentShaderHandle?.Dispose();
    }
}