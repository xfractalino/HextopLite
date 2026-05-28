using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.UI.Composition;
using HextopLite.engine.windows;
using HextopLite.interop;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace HextopLite.engine;

/// <summary>
///   Main class responsible for the whole rendering lifecycle. It creates, employs and destroys graphics objects.
/// </summary>
public class RenderingEngine
{
    private volatile int _running;

    private IRenderingContext _renderingContext = null!;
    
    private readonly ManualResetEventSlim _cleanedUpGate = new (false);

    private readonly Settings _settings = Settings.Default;
    
    private RenderingMetrics _metrics;

    public static RenderingEngine Instance
    {
        get
        {
            field ??= new RenderingEngine();
            
            return field;
        }
    }

    public void Stop()
    {
        Interlocked.Exchange(ref _running, 0);
    }

    public void Start()
    {
        Interlocked.Exchange(ref _running, 1);

        var renderer = new Thread(RunWithChecks);
        renderer.SetApartmentState(ApartmentState.STA);
        renderer.Start();
    }

    public void WaitUntilTermination()
    {
        if (Interlocked.CompareExchange(ref _running, 1, 1) == 0)
        {
            Console.WriteLine("Warning: the renderer is not currently running. Calling WaitUntilTermination at this " +
                              "point is almost certainly an error.");

            return;
        }
        
        _cleanedUpGate.Wait();
    }

    private void Init()
    {
        var platform = _settings.Platform;

        switch (platform)
        {
            case Platform.Windows:
                _renderingContext = new WindowsRenderingContext();
                break;
            default:
                throw new PlatformNotSupportedException("The current platform is not supported.");
        }
        
        _renderingContext.Initialize(_settings.RendererType);
    }

    private void RunWithChecks()
    {
        try
        {
            Run();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.ToString());
#if DEBUG
            _renderingContext.DebugCheck();
#endif
            Stop();
            Dispose();
        }
        finally
        {
            _cleanedUpGate.Set();
        }
    }

    private void Run()
    {
        Init();
        
        _metrics = new RenderingMetrics();
        _metrics.TimerStopwatch.Start();
        
        while (Interlocked.CompareExchange(ref _running, 1, 0) != 0 && _renderingContext.IsValid())
        {
            _renderingContext.PreRender();
            _renderingContext.Render();
            
            var hr = DwmInterop.DCompositionWaitForCompositorClock(0, null, ~0u);
            Marshal.ThrowExceptionForHR(hr);
            
            _metrics.SnapshotMetrics();
        }
        
        Console.Write("Out of the render loop. ");

        Dispose();
    }
    
    private void Dispose()
    {
        Console.WriteLine("Now cleaning up.");

        // D3D resources
        _renderingContext.Dispose();
    }
    
    public sealed class RenderingMetrics
    {
        internal readonly Stopwatch TimerStopwatch = new();
        public TimeSpan FrameTime { get; internal set; }

        public double Fps => 1.0 / FrameTime.TotalSeconds;

        internal void SnapshotMetrics()
        {
            FrameTime = TimerStopwatch.Elapsed;
            TimerStopwatch.Restart();
        }
    }
}