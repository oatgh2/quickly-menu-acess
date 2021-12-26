using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static RegistryManager.Enum.EnumRegHelper;

namespace RegistryManager.RegManager
{
  public static class RegistryHelper
  {
    public static void RegInitializeWithWin(ToDo todo)
    {
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

    public static string RegContextWindowsMenu(ToDo todo, string iconPath, string description)
    {
      RegistryKey keyContextMenuWindowsExecuteShell = Registry.ClassesRoot.OpenSubKey("*").OpenSubKey("shell", true);
      RegistryKey regContextInit = keyContextMenuWindowsExecuteShell.OpenSubKey("Open with QuicklyMenu", true);
      if(regContextInit == null && todo != ToDo.Create)
      {
        throw new Exception("Error, the key does not exists");
      }
      switch (todo)
      {
        case ToDo.Create:
          
          if (regContextInit == null)
          {

            keyContextMenuWindowsExecuteShell.CreateSubKey(@"Open with QuicklyMenu");
            regContextInit = keyContextMenuWindowsExecuteShell.OpenSubKey("Open with QuicklyMenu", true);
            regContextInit.SetValue("", description);
            regContextInit.SetValue("Icon", iconPath);
            regContextInit.CreateSubKey("command");
            RegistryKey regContextCommandInit = regContextInit.OpenSubKey("command", true);
            string local = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            regContextCommandInit.SetValue("", local + "\\");
            return "Registry Created";
          }
          else
          {
            return "Registry Already Exists";
          }
        case ToDo.Delete:
          keyContextMenuWindowsExecuteShell.DeleteSubKey("Open with QuicklyMenu");
          return "Registry Deleted";
        default:
          throw new Exception("Error, enum not exists");
      }
    }
  }
}
