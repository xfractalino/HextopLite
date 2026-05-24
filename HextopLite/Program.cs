using HextopLite.engine;

namespace HextopLite;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hextop Lite - (C) 2026 Francesco Sollazzi");
        
        Renderer.Instance.Start();
    }
}