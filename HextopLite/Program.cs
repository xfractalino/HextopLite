using HextopLite.engine;

namespace HextopLite;

class Program
{
    private static void Main(string[] args)
    {
        _ = args;
        
        Console.WriteLine("Hextop Lite - (C) 2026 Francesco Sollazzi");
        
        Console.CancelKeyPress += (_, _) => Renderer.Instance.Stop();
        
        Renderer.Instance.Start();
    }
}