using AppBarMenu.Controllers;
using AppBarMenu.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AppBarMenu
{
  public partial class App : Application
  {
    Configuration _configuration { get; set; }
    public App()
    {
      _configuration = new Configuration();
      _configuration.Initialize();
    }


    void App_Startup(object sender, StartupEventArgs e)
    {
      Process currentProcess = Process.GetCurrentProcess();
      Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
      Process process = processes.FirstOrDefault(p => p.Id != currentProcess.Id);
      TcpClient tcpClient = null;
      if (process != null)
      {
        tcpClient = new TcpClient();
        tcpClient.Connect("127.0.0.1", 25762);
      }

      if (e.Args.Length == 0)
      {
        if (process != null)
        {
          try
          {

            NetworkStream networkStream = tcpClient.GetStream();
            byte[] mensagemCodificada = Encoding.UTF8.GetBytes("show-main-window");
            networkStream.Write(mensagemCodificada, 0, mensagemCodificada.Length);
            tcpClient.Close();
            Application.Current.Shutdown();
          }
          catch (Exception ex)
          {
            Application.Current.Shutdown();
          }
        }
        else
        {
          MainWindow mainWindow = new MainWindow(_configuration);
          new MainWindowSocketController(mainWindow).Start();
        }
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
        if (process != null)
        {
          try
          {


            if (!_configuration["OpenWhenAddByContextMenu"])
            {
              NetworkStream networkStream = tcpClient.GetStream();
              byte[] mensagemCodificada = Encoding.UTF8.GetBytes("refresh-itens-window");
              networkStream.Write(mensagemCodificada, 0, mensagemCodificada.Length);
            }
            else
            {
              NetworkStream networkStream = tcpClient.GetStream();
              byte[] mensagemCodificada = Encoding.UTF8.GetBytes("refresh-itens-window-open");
              networkStream.Write(mensagemCodificada, 0, mensagemCodificada.Length);
            }


            tcpClient.Close();
          }
          catch (Exception ex)
          {
          }
        }
        Application.Current.Shutdown();
      }
    }
  }
}
