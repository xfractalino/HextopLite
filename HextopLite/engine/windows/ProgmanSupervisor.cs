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

    public IntPtr ShellViewHwnd
    {
        get
        {
            if (field == IntPtr.Zero)
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
        
        if (ProgmanHwnd == IntPtr.Zero)
            throw new Exception("Progman window not found on this system.");
    }

    private void FindWorkerW()
    {
        WorkerWHwnd = User32.FindWindowEx(ProgmanHwnd, IntPtr.Zero, "WorkerW", null!);
    }

    private void FindShellView()
    {
        ShellViewHwnd = User32.FindWindowEx(ProgmanHwnd, IntPtr.Zero, "SHELLDLL_DefView", null!);
    }
}