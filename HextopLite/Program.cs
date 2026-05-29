using HextopLite.engine;

namespace HextopLite;

class Program
{
    private static void Main(string[] args)
    {
        _ = args;
        
        Console.WriteLine("Hextop Lite - (C) 2026 xfractalino");

        Console.CancelKeyPress += (_, _) => OnCancelKeyPress();
        
        RenderingEngine.Instance.Start();
        RenderingEngine.Instance.WaitUntilTermination();
    }

    private static void OnCancelKeyPress()
    {
        RenderingEngine.Instance.Stop();
        RenderingEngine.Instance.WaitUntilTermination();
    }
}