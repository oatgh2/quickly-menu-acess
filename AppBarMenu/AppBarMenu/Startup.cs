using AppBarMenu.Controllers;
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
      if (e.Args.Length == 0)
      {
        MainWindow mainWindow = new MainWindow();
      }
      else
      {
        string filepath = e.Args[0];
        FilesController filesController = new FilesController();
        string extension = System.IO.Path.GetExtension(filepath);
        string mimmeType = System.Web.MimeMapping.GetMimeMapping(filepath);
        Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
        fileDialog.FileName = filepath;
        filesController.Add(new Entities.Entidades.FileModel()
        {
          Id = Guid.NewGuid(),
          Name = fileDialog.SafeFileName,
          Path = fileDialog.FileName,
          Extension = extension,
          MimmeType = mimmeType
        });
        Application.Current.Shutdown();
      }
    }
  }
}
