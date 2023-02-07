using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static RegistryManager.Enum.EnumRegHelper;

namespace RegistryManager.RegManager
{
  public static class RegistryHelper
  {

    private static bool isRunningAsAdm()
    {
      try
      {
        WindowsIdentity id = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(id);
        bool result = principal.IsInRole(WindowsBuiltInRole.Administrator);
        return result;
      }
      catch (Exception ex)
      {
        return false;
      }
    }



    public static void RegInitializeWithWin(ToDo todo)
    {
      if (!isRunningAsAdm())
        RunAsAdm();

      RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
      if (key == null && todo == ToDo.Delete)
      {
        throw new Exception("Error, the key does not exists");
      }
      switch (todo)
      {
        case ToDo.Create:
          string local = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
          object regInit = key.GetValue("QuickStartMenu");
          if (regInit == null)
            key.SetValue("QuickStartMenu", local + "\\QuickStartMenu.exe");
          break;
        case ToDo.Delete:
          key.DeleteValue("QuickStartMenu");
          break;
      }
    }

    public static void RunAsAdm()
    {
      ProcessStartInfo proc = new ProcessStartInfo();
      proc.UseShellExecute = true;
      proc.WorkingDirectory = Environment.CurrentDirectory;
      proc.FileName = Assembly.GetEntryAssembly().Location;
      proc.Arguments = "--just-run";
      proc.Verb = "runas";

      try
      {
        Process.Start(proc);
        Environment.Exit(0);
      }
      catch (Exception ex)
      {
        Console.WriteLine("This program must be run as an administrator! \n\n" + ex.ToString());
      }
    }

    public static string RegContextWindowsMenu(ToDo todo, string iconPath, string description)
    {
      if (!isRunningAsAdm())
        RunAsAdm();
      RegistryKey keyContextMenuWindowsExecuteShell = Registry.ClassesRoot.OpenSubKey("*").OpenSubKey("shell", true);
      RegistryKey regContextInit = keyContextMenuWindowsExecuteShell.OpenSubKey("Open with QuicklyMenu", true);
      if (regContextInit == null && todo != ToDo.Create)
      {
        throw new Exception("Error, the key does not exists");
      }
      switch (todo)
      {
        case ToDo.Create:


          keyContextMenuWindowsExecuteShell.CreateSubKey(@"Open with QuicklyMenu");
          regContextInit = keyContextMenuWindowsExecuteShell.OpenSubKey("Open with QuicklyMenu", true);
          regContextInit.SetValue("", description);
          regContextInit.SetValue("Icon", iconPath);
          regContextInit.CreateSubKey("command");
          RegistryKey regContextCommandInit = regContextInit.OpenSubKey("command", true);
          string local = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
          regContextCommandInit.SetValue("", "\"" + local + @"\QuickStartMenu.exe" + "\"" + @" ""%1""");
          return "Registry Created";

        case ToDo.Delete:
          keyContextMenuWindowsExecuteShell.DeleteSubKey("Open with QuicklyMenu");
          return "Registry Deleted";
        default:
          throw new Exception("Error, enum not exists");
      }
    }
  }
}
