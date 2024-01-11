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
                    var toHide = new List<(string title, WindowHelper.RECT rect, IntPtr hwnd)>();
                    WindowHelper.EnumWindows(delegate(IntPtr wnd, IntPtr param)
                    {
                        try
                        {
                            var containsClass = WindowHelper.GetClassName(wnd).Contains("TeamsWebView");
                            if (!containsClass) return true;
                            var wdwText = WindowHelper.GetWindowText(wnd);
                            var containsTitle = wdwText.Contains("| Microsoft Teams");
                            if (!containsTitle) return true;
                            WindowHelper.GetWindowRect(wnd, out var rect);
                            toHide.Add((wdwText, rect, wnd));
                        }
                        catch
                        {
                            // ignored
                        }

                        return true;

                    }, IntPtr.Zero);

                    if (Config.GetWindows)
                    {
                        var content = string.Join(Environment.NewLine, toHide.Select(x => x.title));
                        await File.WriteAllTextAsync(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Windows.txt"),
                            content);
                        MessageBox.Show("Done");
                        Application.Exit();
                    }

                    toHide = toHide.SelectMany(x =>
                                x.title.Split("|").FirstOrDefault()
                                    .Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                                    .ToList(),
                            (x, y) => (y, x.rect, x.hwnd))
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
                                        (firstItem.title.Contains("Freigabesteuerungsleiste") ||
                                        firstItem.title.Contains("Sharing control bar") ||
                                        (!string.IsNullOrWhiteSpace(Config.TopBarWindowName) && firstItem.title.Contains(Config.TopBarWindowName))))
                                    {
                                        WindowHelper.ShowWindow((int)firstItem.hwnd, WindowHelper.SW_HIDE);
                                    }

                                    break;
                                }
                                case > 1:
                                {
                                    if (Config.HideBottomOverlay)
                                    {
                                        var smallestItem = list.OrderBy(x => x.rect.Surface)
                                            .FirstOrDefault(x => !WindowHelper.IsIconic(x.hwnd));
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

