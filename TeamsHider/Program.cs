using System.Reflection;

namespace TeamsHider
{
    static class Program
    {
        private static readonly string AppId = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            using var mutex = new Mutex(false, AppId);
            if (!mutex.WaitOne(0, false))
            {
                return;
            }

            // Create a new NotifyIcon instance.
            using var tray = new TrayApplicationContext();
            Task.Run(async () =>
            {
                while (true)
                {
                    var toHide = new List<(string title, WindowHelper.DisplayAffinity affinity, IntPtr hwnd)>();
                    WindowHelper.EnumWindows(delegate(IntPtr wnd, IntPtr param)
                    {
                        try
                        {
                            var containsClass = WindowHelper.GetClassName(wnd).Contains("TeamsWebView");
                            if (!containsClass) return true;
                            var wdwText = WindowHelper.GetWindowText(wnd);
                            var containsTitle = wdwText.Contains("| Microsoft Teams");
                            if (!containsTitle) return true;
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
                                x.title.Split("|").FirstOrDefault()
                                    .Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                                    .ToList(),
                            (x, y) => (y, x.affinity, x.hwnd))
                        .ToList();
                    
                    foreach (var list in toHide.GroupBy(x => x.title))
                    {
                        try
                        {
                            switch (list.Count())
                            {
                                case 1:
                                {
                                    var firstItem = list.FirstOrDefault();
                                    if (Config.HideTopBar &&
                                        firstItem.affinity is WindowHelper.DisplayAffinity.Monitor or WindowHelper.DisplayAffinity.ExcludeFromCapture)
                                    {
                                        WindowHelper.ShowWindow((int)firstItem.hwnd, WindowHelper.SW_HIDE);
                                    }

                                    break;
                                }
                                case > 1:
                                {
                                    if (Config.HideBottomOverlay)
                                    {
                                        var smallestItem = list.FirstOrDefault(x =>
                                            x.affinity is WindowHelper.DisplayAffinity.Monitor
                                                or WindowHelper.DisplayAffinity.ExcludeFromCapture);
                                        WindowHelper.ShowWindow((int)smallestItem.hwnd, WindowHelper.SW_HIDE);
                                    }
                                    break;
                                }
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
            Application.Run();
        }
    }
}

