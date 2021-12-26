using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AppBarMenu
{
  public partial class App : Application
  {
    void App_Startup(object sender, StartupEventArgs e)
    {
      if (string.IsNullOrEmpty(e.Args[0]))
      {
        MainWindow mainWindow = new MainWindow();
      }
      else
      {

      }
    }
  }
}
