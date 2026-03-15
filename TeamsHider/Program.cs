namespace TeamsHider;

static class Program
{
    private static readonly string AppId = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    private const string TeamsWindowClass = "TeamsWebView";

    private static readonly (string Prefix, Func<Config, bool> Enabled)[] HidePatterns =
    [
        ("Meeting compact view | ", c => c.HideBottomOverlay),
        ("Sharing control bar | ", c => c.HideTopBar),
    ];

    // prevent GC collection of the delegate
    private static readonly WindowHelper.WinEventDelegate WinEventCallback = OnWinEvent;

    private static IntPtr _hookShow;
    private static IntPtr _hookNameChange;

    [STAThread]
    private static void Main(string[] args)
    {
        using var mutex = new Mutex(false, AppId);
        if (!mutex.WaitOne(0, false))
        {
            return;
        }

        using var tray = new TrayApplicationContext();

        // Initial scan for already-open Teams windows
        WindowHelper.EnumWindows(delegate(IntPtr wnd, IntPtr param)
        {
            TryHide(wnd);
            return true;
        }, IntPtr.Zero);

        // Install event hooks on the UI thread
        _hookShow = WindowHelper.SetWinEventHook(
            WindowHelper.EVENT_OBJECT_SHOW, WindowHelper.EVENT_OBJECT_SHOW,
            IntPtr.Zero, WinEventCallback, 0, 0, WindowHelper.WINEVENT_OUTOFCONTEXT);

        _hookNameChange = WindowHelper.SetWinEventHook(
            WindowHelper.EVENT_OBJECT_NAMECHANGE, WindowHelper.EVENT_OBJECT_NAMECHANGE,
            IntPtr.Zero, WinEventCallback, 0, 0, WindowHelper.WINEVENT_OUTOFCONTEXT);

        // Periodic config reload
        var configTimer = new System.Windows.Forms.Timer { Interval = 5000 };
        configTimer.Tick += (_, _) => Config.ReloadIfChanged();
        configTimer.Start();

        Application.ApplicationExit += (_, _) =>
        {
            configTimer.Stop();
            if (_hookShow != IntPtr.Zero) WindowHelper.UnhookWinEvent(_hookShow);
            if (_hookNameChange != IntPtr.Zero) WindowHelper.UnhookWinEvent(_hookNameChange);
        };

        Application.Run(tray);
    }

    private static void OnWinEvent(
        IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
        int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (idObject != WindowHelper.OBJID_WINDOW || idChild != 0)
            return;

        TryHide(hwnd);
    }

    private static void TryHide(IntPtr hwnd)
    {
        try
        {
            if (!WindowHelper.GetClassName(hwnd).Contains(TeamsWindowClass))
                return;

            var title = WindowHelper.GetWindowText(hwnd);
            foreach (var pattern in HidePatterns)
            {
                if (title.StartsWith(pattern.Prefix) && pattern.Enabled(Config.Instance))
                {
                    WindowHelper.ShowWindow(hwnd, WindowHelper.SW_HIDE);
                    return;
                }
            }
        }
        catch
        {
            // ignored — window may have been destroyed between event and processing
        }
    }
}
