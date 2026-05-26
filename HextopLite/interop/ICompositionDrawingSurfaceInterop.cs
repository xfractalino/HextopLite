using System.Runtime.InteropServices;

namespace HextopLite.interop;

[ComImport]
[Guid("FD04E6E3-FE0C-4C3C-AB19-A07601A576EE")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICompositionDrawingSurfaceInterop
{
    void BeginDraw(nint updateRect, ref Guid iid, out nint updateObject, out InteropCommons.POINT offset);
    void EndDraw();
}