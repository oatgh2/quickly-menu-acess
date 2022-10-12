using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AppBarMenu.Helpers
{
  public static class WinApiHelper
  {
    public const int SW_HIDE = 0;
    public const int SW_SHOW = 5;
    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindow(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int ShowWindow(int hwnd, int nCmdShow);

    [DllImport("user32.dll")]
    static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
        IntPtr lParam);

    public static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
    {
      var handles = new List<IntPtr>();

      foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
        EnumThreadWindows(thread.Id,
            (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

      return handles;
    }

  }
}
