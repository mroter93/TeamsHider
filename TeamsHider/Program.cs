namespace TeamsHider;

static class Program
{
    private static readonly string AppId = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

    private const string TeamsWindowClass = "TeamsWebView";
    private const string TeamsWindowTitle = "| Microsoft Teams";
    private const string BottomOverlayText = "Meeting compact view";

    [STAThread]
    private static void Main(string[] args)
    {
        using var mutex = new Mutex(false, AppId);
        if (!mutex.WaitOne(0, false))
        {
            return;
        }

        using var tray = new TrayApplicationContext();
        Task.Run(async () =>
        {
            while (true)
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

                await Task.Delay(2000);
            }
        });
        Application.Run(tray);
    }
}
