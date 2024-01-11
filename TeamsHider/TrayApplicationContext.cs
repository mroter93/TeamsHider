using System.Diagnostics;

namespace TeamsHider;

using System;
using System.Drawing;
using System.Windows.Forms;

class TrayApplicationContext : Form
{
    private NotifyIcon notifyIcon;
    
    public TrayApplicationContext()
    {
        base.InitLayout();
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
            // Opens the URL in the default browser.
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
            Application.Exit();
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing && notifyIcon != null)
        {
            notifyIcon.Dispose();
        }
        
        base.Dispose(disposing);
    }
}