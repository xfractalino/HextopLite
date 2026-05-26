using HextopLite.engine;

namespace HextopLite;

class Program
{
    private static void Main(string[] args)
    {
        _ = args;
        
        Console.WriteLine("Hextop Lite - (C) 2026 Francesco Sollazzi");

        Console.CancelKeyPress += (_, _) => OnCancelKeyPress();
        
        Renderer.Instance.Start();
        Renderer.Instance.WaitUntilTermination();
    }

    private static void OnCancelKeyPress()
    {
        Renderer.Instance.Stop();
        Renderer.Instance.WaitUntilTermination();
    }
}