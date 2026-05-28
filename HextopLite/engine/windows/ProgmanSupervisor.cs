using HextopLite.interop;

namespace HextopLite.engine.windows;

public class ProgmanSupervisor
{
    public const string ProgmanName = "Progman";
    public const int MagicMessage = 0x052c;

    private ProgmanSupervisor()
    {
        Magic();
        
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

    public nint ProgmanHwnd
    {
        get
        {
            if (field == 0)
                FindHwnd();
        
            return field;
        }

        private set;
    }

    public nint WorkerWHwnd
    {
        get
        {
            if (field == 0)
                FindWorkerW();
        
            return field;
        }

        private set;
    }

    public nint ShellViewHwnd
    {
        get
        {
            if (field == 0)
                FindShellView();

            return field;
        }
        private set;
    }

    /// <summary>
    ///   Invokes an undocumented message for progman that spawns a fresh WorkerW window behind the icons.
    /// </summary>
    private void Magic()
    {
        User32.SendMessage(ProgmanHwnd, MagicMessage, 0xD, 0x1);
    }

    private void FindHwnd()
    {
        ProgmanHwnd = User32.FindWindow(ProgmanName, null!);
        
        if (ProgmanHwnd == 0)
            throw new Exception("Progman window not found on this system.");
    }

    private void FindWorkerW()
    {
        WorkerWHwnd = User32.FindWindowEx(ProgmanHwnd, 0, "WorkerW", null!);
    }

    private void FindShellView()
    {
        ShellViewHwnd = User32.FindWindowEx(ProgmanHwnd, 0, "SHELLDLL_DefView", null!);
    }
}