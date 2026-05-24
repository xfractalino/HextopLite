using HextopLite.interop;

namespace HextopLite.engine;

public class ProgmanSupervisor
{
    public const string ProgmanName = "Progman";
    public const int MagicMessage = 0x052c;

    private ProgmanSupervisor()
    {
        Console.WriteLine("New session of Progman Supervisor started.");
    }

    public static ProgmanSupervisor Instance
    {
        get
        {
            field ??= new ProgmanSupervisor();

            return field;
        }
    } = null;

    public IntPtr ProgmanHwnd
    {
        get
        {
            if (field == IntPtr.Zero)
                FindHwnd();
        
            return field;
        }

        private set;
    }

    public IntPtr WorkerWHwnd
    {
        get
        {
            if (field == IntPtr.Zero)
                FindWorkerW();
        
            return field;
        }

        private set;
    }

    private void FindHwnd()
    {
        ProgmanHwnd = User32.FindWindow(ProgmanName, null!);
        
        if (ProgmanHwnd == IntPtr.Zero)
            throw new Exception("Progman window not found on this system.");
    }

    private void FindWorkerW()
    {
        User32.SendMessage(ProgmanHwnd, MagicMessage, IntPtr.Zero, IntPtr.Zero);
        
        WorkerWHwnd = User32.FindWindowEx(ProgmanHwnd, IntPtr.Zero, "WorkerW", null!);
    }
}