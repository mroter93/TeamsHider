using System.Diagnostics;

namespace TeamsHider;

class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon notifyIcon;

    public TrayApplicationContext()
    {
        notifyIcon = new NotifyIcon
        {
            Icon = new Icon("invisible.ico"),
            Visible = true,
        };
        notifyIcon.ContextMenuStrip = new ContextMenuStrip();
        notifyIcon.ContextMenuStrip.Items.Add("About", null, OnAboutClicked);
        notifyIcon.ContextMenuStrip.Items.Add("Quit", null, OnQuitClicked);

        notifyIcon.MouseClick += (sender, args) =>
        {
            notifyIcon.ContextMenuStrip.Show();
        };
    }

    private void OnAboutClicked(object? sender, EventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/mroter93/TeamsHider",
                UseShellExecute = true
            });
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void OnQuitClicked(object? sender, EventArgs e)
    {
        var res = MessageBox.Show("Quit?", "", MessageBoxButtons.YesNo);
        if (res == DialogResult.Yes)
        {
            notifyIcon.Visible = false;
            ExitThread();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            notifyIcon.Dispose();
        }

        base.Dispose(disposing);
    }
}
