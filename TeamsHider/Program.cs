namespace TeamsHider;

static class Program
{
    private static readonly string AppId = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    private const string TeamsWindowClass = "TeamsWebView";
    private const string TeamsWindowTitle = "| Microsoft Teams";
    private const string BottomOverlayText = "Meeting compact view";

    private static System.Windows.Forms.Timer _debounceTimer = null!;

    // Must be kept as a static field to prevent GC collection of the delegate
    private static readonly WindowHelper.WinEventDelegate WinEventCallback = OnWinEvent;

    [STAThread]
    private static void Main(string[] args)
    {
        using var mutex = new Mutex(false, AppId);
        if (!mutex.WaitOne(0, false))
        {
            return;
        }

        using var tray = new TrayApplicationContext();

        _debounceTimer = new System.Windows.Forms.Timer { Interval = 200 };
        _debounceTimer.Tick += (_, _) =>
        {
            _debounceTimer.Stop();
            ScanAndHideTeamsWindows();
        };

        // Initial scan to catch already-open Teams windows
        ScanAndHideTeamsWindows();

        // Hook window create, show, and name-change events (system-wide, out-of-context)
        var hook = WindowHelper.SetWinEventHook(
            WindowHelper.EVENT_OBJECT_CREATE,
            WindowHelper.EVENT_OBJECT_NAMECHANGE,
            IntPtr.Zero, WinEventCallback,
            0, 0, WindowHelper.WINEVENT_OUTOFCONTEXT);

        Application.Run(tray);

        WindowHelper.UnhookWinEvent(hook);
    }

    private static void OnWinEvent(
        IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
        int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        // Only care about window-level events (OBJID_WINDOW == 0)
        if (idObject != 0) return;

        // Restart debounce timer — batches rapid events into a single scan
        _debounceTimer.Stop();
        _debounceTimer.Start();
    }

    private static void ScanAndHideTeamsWindows()
    {
        Config.ReloadIfChanged();

        var toHide = new List<(string title, WindowHelper.DisplayAffinity affinity, IntPtr hwnd)>();
        WindowHelper.EnumWindows(delegate(IntPtr wnd, IntPtr param)
        {
            try
            {
                if (!WindowHelper.GetClassName(wnd).Contains(TeamsWindowClass))
                    return true;
                var wdwText = WindowHelper.GetWindowText(wnd);
                if (!wdwText.Contains(TeamsWindowTitle))
                    return true;
                WindowHelper.GetWindowDisplayAffinity(wnd, out var affinity);
                toHide.Add((wdwText, affinity, wnd));
            }
            catch
            {
                // ignored
            }

            return true;
        }, IntPtr.Zero);

        toHide = toHide.SelectMany(x =>
                        (x.title.Split("|").FirstOrDefault() ?? "")
                            .Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                            .ToList(),
                    (x, y) => (y, x.affinity, x.hwnd))
                .ToList();

        foreach (var list in toHide.GroupBy(x => x.title))
        {
            try
            {
                var firstItem = list.FirstOrDefault();
                if (firstItem.title is BottomOverlayText && Config.Instance.HideBottomOverlay)
                {
                    var smallestItem = list.FirstOrDefault(x =>
                        x.affinity is WindowHelper.DisplayAffinity.Monitor
                            or WindowHelper.DisplayAffinity.ExcludeFromCapture);
                    WindowHelper.ShowWindow(smallestItem.hwnd, WindowHelper.SW_HIDE);
                    continue;
                }

                if (Config.Instance.HideTopBar && list.Count() > 1 &&
                    firstItem.affinity is WindowHelper.DisplayAffinity.Monitor
                        or WindowHelper.DisplayAffinity.ExcludeFromCapture)
                {
                    WindowHelper.ShowWindow(firstItem.hwnd, WindowHelper.SW_HIDE);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
