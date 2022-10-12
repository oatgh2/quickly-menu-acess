using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppBarMenu.Controllers
{
  public class MainWindowSocketController
  {
    MainWindow mainWindow;
    public MainWindowSocketController(MainWindow mainWindow)
    {
      this.mainWindow = mainWindow;
    }

    TcpListener ipcCommunication;
    private Thread acceptClientsTrhead;
    private void AccetpClients()
    {
      while (true)
      {
        try
        {
          TcpClient tcpClient = ipcCommunication.AcceptTcpClient();
          NetworkStream clientStream = tcpClient.GetStream();
          byte[] buffer = new byte[1024];
          clientStream.Read(buffer, 0, buffer.Length);
          string decodedMessage = Encoding.UTF8.GetString(buffer.Where(x => !x.Equals(0)).ToArray());
          if (decodedMessage.Equals("show-main-window"))
          {
            Action action = () => mainWindow.Show();
            mainWindow.Dispatcher.Invoke(action);
          }

          if (decodedMessage.Equals("refresh-itens-window"))
          {
            Action action = () => mainWindow.RefrashReoloadList();
            mainWindow.Dispatcher.Invoke(action);
          }

          if (decodedMessage.Equals("refresh-itens-window"))
          {
            Action action = () => mainWindow.RefrashReoloadList();
            mainWindow.Dispatcher.Invoke(action);
          }

          if (decodedMessage.Equals("refresh-itens-window-open"))
          {
            Action action = () => mainWindow.RefrashReoloadList();
            mainWindow.Dispatcher.Invoke(action);

            Action actionOpen = () => mainWindow.Show();
            mainWindow.Dispatcher.Invoke(actionOpen);
          }

          if (decodedMessage.Equals("add-new-item"))
          {
            string[] splittedInfos = decodedMessage.Split('_');
            string filepath = splittedInfos[1];
            if (splittedInfos.Length > 1) 
            {
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
              Action action = () => mainWindow.RefrashReoloadList();
              mainWindow.Dispatcher.Invoke(action);
              
            }
          }
          
          tcpClient.Close();
        }
        catch (Exception ex)
        {

        }
      }
    }
    
    public void Start()
    {
      ipcCommunication = new TcpListener(IPAddress.Parse("127.0.0.1"), 25762);
      ipcCommunication.Start();
      acceptClientsTrhead = new Thread(() => AccetpClients());
      acceptClientsTrhead.Start();
    }
  }
}
