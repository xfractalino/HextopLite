using System.Drawing;
using Vortice.Mathematics;

namespace HextopLite.engine;

public interface IRenderer : IRenderingResource
{
    public enum Topology
    {
        Undefined = 0,
        PointList = 1,
        LineList = 2,
        LineStrip = 3,
        TriangleList = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
    }

    public enum CullMode
    {
        None,
        Front,
        Back,
    }

    public enum FillMode
    {
        Wireframe,
        Solid
    }

    public IShader CreateShader(string path);
    public void SetRasterizerDescription(CullMode cullMode, FillMode fillMode);
    public void Draw(uint n, uint i);
    public void SetTopology(Topology topology);
    public void SetViewport(int x, int y, uint width, uint height);
    public void Clear(Color4 color);
    public void BeginDraw();
    public void EndDraw();
}