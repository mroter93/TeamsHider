using System.Runtime.InteropServices;
using System.Text;

namespace TeamsHider;

public static class WindowHelper
{
    public enum DisplayAffinity : uint
    {
        None = 0x00,
        Monitor = 0x01,
        ExcludeFromCapture = 0x11
    }

    [DllImport("user32.dll")]
    public static extern bool GetWindowDisplayAffinity(IntPtr hwnd, out DisplayAffinity affinity);

    public const int SW_HIDE = 0;

    [DllImport("User32")]
    public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public static string GetWindowText(IntPtr hWnd)
    {
        int size = GetWindowTextLength(hWnd);
        if (size > 0)
        {
            var builder = new StringBuilder(size + 1);
            GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        return string.Empty;
    }

    public static string GetClassName(IntPtr hWnd)
    {
        var builder = new StringBuilder(256);
        GetClassName(hWnd, builder, builder.Capacity);
        return builder.ToString();
    }
}
