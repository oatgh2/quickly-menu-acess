using AppBarMenu.Controllers;
using AppBarMenu.Helpers;
using Entities.Entidades;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Reflection;
using System.Buffers.Text;

namespace AppBarMenu
{
  /// <summary>
  /// Interação lógica para MainWindow.xam
  /// </summary>
  public partial class MainWindow : Window
  {
    private ItemsControl _itemContextMenuRemove = new ItemsControl();
    private ItemsControl _itemContextMenuDetails = new ItemsControl();
    private ContextMenu _contextMenu = new ContextMenu();
    private FilesController _controller;
    private List<FileModel> _listModels;
    private Forms.NotifyIcon trayBar = new Forms.NotifyIcon();
    private Drawing.Image imagem_Delete;
    private Drawing.Image imagem_Details;
    private Drawing.Image imagem_Exclude;
    private Drawing.Image imagem_OpenFile;
    private Drawing.Image imagem_Open;
    public MainWindow()
    {
      InitializeComponent();
      InitialConfigs();
      this.Hide();
      
    }

    

    private void InitialConfigs()
    {
      try
      {
        _controller = new FilesController();
        ReloadList();
        ListaDeItensBox.AllowDrop = true;
        trayBar.Icon = new System.Drawing.Icon("favicon.ico");
        trayBar.Visible = true;
        trayBar.DoubleClick += DoubleClickTrayBar;

        string[] imagesString = FileHelper.GetImages();

        for (int i = 0; i < imagesString.Length; i++)
        {
          string image = imagesString[i];
          using (MemoryStream msImg = new MemoryStream(Convert.FromBase64String(image)))
          {
            switch (i)
            {
              case 0:
                imagem_Delete = Drawing.Image.FromStream(msImg);
                break;
              case 1:
                imagem_Details = Drawing.Image.FromStream(msImg);
                break;
              case 2:
                imagem_Exclude = Drawing.Image.FromStream(msImg);
                break;
              case 3:
                imagem_OpenFile = Drawing.Image.FromStream(msImg);
                break;
              case 4:
                imagem_Open = Drawing.Image.FromStream(msImg);
                break;
              case 5:
                break;
              default:
                break;
            }
          }
        }

        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        string local = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        object regInit = key.GetValue("QuickStartMenu");
        if (regInit == null)
          key.SetValue("QuickStartMenu", local + "\\QuickStartMenu.exe");




      }
      catch (Exception ex)
      {
        MessageBox.Show("Não foi possível carregar o aplicativo!");
        this.Close();
      }

      ////Remove Btn
      //_itemContextMenuRemove.MouseLeftButtonUp += ItemControlRemove_Click;
      //_itemContextMenuRemove.Items.Add("Remover");

      ////Detalhes Btn
      //_itemContextMenuRemove.MouseLeftButtonUp += ItemControlDetails_Click;
      //_itemContextMenuDetails.Items.Add("Detalhes");

      //_contextMenu.Items.Add(_itemContextMenuRemove);
      //_contextMenu.Items.Add(_itemContextMenuDetails);
    }

    void DoubleClickTrayBar(object sender, EventArgs e)
    {
      this.Show();
    }
    void ClickTraybarOpen(object sender, EventArgs e)
    {
      try
      {
        Forms.ToolStripButton entidade = sender as Forms.ToolStripButton;
        string[] splittedValueName = entidade.AccessibilityObject.Name.Split('.');
        int objValueItemIndex = Int32.Parse(splittedValueName[0]);
        System.Diagnostics.Process.Start(_listModels[objValueItemIndex].Path);

      }
      catch (Exception ex) { }
    }
    void ClickTrayBarOpenMain(object sender, EventArgs e)
    {
      this.Show();

    }

    void ClickTraybarOpenFolder(object sender, EventArgs e)
    {
      Forms.ToolStripButton entidade = sender as Forms.ToolStripButton;

      try
      {
        string[] splittedValueName = entidade.AccessibilityObject.Name.Split('.');
        int objValueItemIndex = Int32.Parse(splittedValueName[0]);
        var item = _listModels[objValueItemIndex];
        System.Diagnostics.Process.Start("explorer.exe", item.Path.Replace(item.Name, string.Empty));
      }
      catch (Exception ex) { }
    }

    void ReloadList()
    {
      _listModels = _controller.GetFiles();
      ListaDeItensBox.Items.Clear();
      JumpList jumpList = new JumpList();
      foreach (FileModel item in _listModels)
      {
        ListaDeItensBox.Items.Add(item.Name);
        string name = string.IsNullOrEmpty(item.Extension) ? item.Name : item.Name.Replace(item.Extension, string.Empty);
        JumpTask jumpTask = new JumpTask();
        jumpTask.ApplicationPath = item.Path;
        jumpTask.IconResourcePath = item.Path;
        jumpTask.Title = name;
        jumpTask.Description = "Abrir " + item.Name;
        jumpTask.CustomCategory = "Adcionados TOP(10)";
        jumpTask.WorkingDirectory = item.Path;
        jumpList.JumpItems.Add(jumpTask);
      }
      JumpList.SetJumpList(App.Current, jumpList);
      trayBar.ContextMenuStrip = new Forms.ContextMenuStrip();
      trayBar.ContextMenuStrip.Items.Add(new
      Forms.ToolStripButton("Abrir Aplicação", imagem_Open, ClickTrayBarOpenMain));
      trayBar.ContextMenuStrip.Items.Add(new
      Forms.ToolStripButton("Adcionar Novo Arquivo", imagem_OpenFile, AddItemTrayBarButton));
      for (int i = 0; i < _listModels.Count; i++)
      {
        var item = _listModels[i];
        //MenuItem menuItem = new MenuItem();
        //menuItem.Header = string.IsNullOrEmpty(item.Extension) ? item.Name : item.Name.Replace(item.Extension, string.Empty);
        //menuItem.Icon = item.Path;
        Drawing.Image imagemIcon;
        try
        {
          if (!string.IsNullOrEmpty(item.Extension))
            imagemIcon = Drawing.Icon.ExtractAssociatedIcon(item.Path).ToBitmap();
          else
            imagemIcon = Drawing.Icon.ExtractAssociatedIcon("explorer.exe").ToBitmap();
        }
        catch (Exception ex)
        {
          imagemIcon = Drawing.Image.FromFile("favicon.ico");

        }
        //string.IsNullOrEmpty(item.Extension) ? item.Name
        //  : item.Name.Replace(item.Extension, string.Empty), imagemIcon, ClickTraybar
        trayBar.ContextMenuStrip.Items.Add(
          new Forms.ToolStripDropDownButton(
            string.IsNullOrEmpty(item.Extension) ? item.Name
          : item.Name.Replace(item.Extension, string.Empty), imagemIcon
          , new Forms.ToolStripButton($"{i}. Abrir", imagem_Open, ClickTraybarOpen)
          , new Forms.ToolStripButton($"{i}. Abrir Pasta", imagem_OpenFile, ClickTraybarOpenFolder))); ;
      }
      trayBar.ContextMenuStrip.Items.Add(new
      Forms.ToolStripButton("Sair", Drawing.Image.FromFile("exclude_icon.png"), TrayBarCloseProgram));
    }

    void TrayBarCloseProgram(object sender, EventArgs e)
    {
      Environment.Exit(0);
    }
    //private void BtnAdd2_Click(object sender, RoutedEventArgs e)
    //{
    //  MessageBox.Show("clicado no xmlamammsasaqs");
    //}
    private void AddItemTrayBarButton(object sender, EventArgs e)
    {
      OpenFileDialog fileDialog = new OpenFileDialog();
      fileDialog.ShowDialog();
      if (!string.IsNullOrEmpty(fileDialog.FileName))
      {
        string extension = System.IO.Path.GetExtension(fileDialog.FileName);
        string mimmeType = MimeMapping.GetMimeMapping(fileDialog.FileName);
        _controller.Add(new FileModel()
        {
          Id = Guid.NewGuid(),
          Name = fileDialog.SafeFileName,
          Path = fileDialog.FileName,
          Extension = extension,
          MimmeType = mimmeType
        });
        ReloadList();
      }
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog fileDialog = new OpenFileDialog();
      fileDialog.ShowDialog();
      if (!string.IsNullOrEmpty(fileDialog.FileName))
      {
        string extension = System.IO.Path.GetExtension(fileDialog.FileName);
        string mimmeType = MimeMapping.GetMimeMapping(fileDialog.FileName);
        _controller.Add(new FileModel()
        {
          Id = Guid.NewGuid(),
          Name = fileDialog.SafeFileName,
          Path = fileDialog.FileName,
          Extension = extension,
          MimmeType = mimmeType
        });
        ReloadList();
      }

    }

    private void ListaDeItensBox_DragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
    }

    private void ListaDeItensBox_DragOver(object sender, DragEventArgs e)
    {
    }

    private void ListaDeItensBox_Drop(object sender, DragEventArgs e)
    {
      string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
      foreach (string item in files)
      {
        OpenFileDialog fileDialog = new OpenFileDialog()
        {
          FileName = item
        };
        string extension = System.IO.Path.GetExtension(fileDialog.FileName);
        string mimmeType = MimeMapping.GetMimeMapping(fileDialog.FileName);
        _controller.Add(new FileModel()
        {
          Id = Guid.NewGuid(),
          Name = fileDialog.SafeFileName,
          Path = fileDialog.FileName,
          Extension = extension,
          MimmeType = mimmeType
        });
      }
      ReloadList();
    }


    private void ItemControlExclude_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (ListaDeItensBox.SelectedIndex != -1)
        {
          var result = MessageBox.Show("Ao confirmar, o arquivo será apagado do seu sistema de arquivos, deseja continuar?",
        "Confirmar?", MessageBoxButton.YesNo, MessageBoxImage.Question);
          if (result == MessageBoxResult.Yes)
          {
            _controller.DeleteFile(ListaDeItensBox.SelectedIndex);
            ReloadList();
          }
        }
        else
        {
          MessageBox.Show("Selecione um item");
        }

      }
      catch (Exception ex)
      {

      }

    }

    private void ItemControlOpen_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (ListaDeItensBox.SelectedIndex != -1)
        {
          System.Diagnostics.Process.Start(_listModels[ListaDeItensBox.SelectedIndex].Path);

        }
        else
        {
          MessageBox.Show("Selecione um item");
        }
      }
      catch (Exception ex) { }

    }
    private void ItemControlOpenFile_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (ListaDeItensBox.SelectedIndex != -1)
        {
          var item = _listModels[ListaDeItensBox.SelectedIndex];
          System.Diagnostics.Process.Start("explorer.exe", item.Path.Replace(item.Name, string.Empty));
        }
        else
        {
          MessageBox.Show("Selecione um item");
        }

      }
      catch (Exception ex) { }
    }


    private void ItemControlRemove_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (ListaDeItensBox.SelectedIndex != -1)
        {
          int itemSelecionado = ListaDeItensBox.SelectedIndex;
          _controller.Remove(itemSelecionado);
          ReloadList();
        }
        else
        {
          MessageBox.Show("Selecione um item");
        }

      }
      catch (Exception ex)
      {

      }

    }


    private void ItemControlDetails_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (ListaDeItensBox.SelectedIndex != -1)
        {
          MessageBox.Show("ClicadoDetails");
        }
        else
        {
          MessageBox.Show("Selecione um item");
        }

      }
      catch (Exception ex)
      {

      }
    }
    private void ListaDeItensBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        if (ListaDeItensBox.SelectedIndex != -1)
        {
          ContextStripListOpen.IsOpen = true;
        }
        else
        {
          MessageBox.Show("Selecione um item");
        }
      }
      catch (Exception ex)
      {

      }

    }

    private void ListaDeItensBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {

    }

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
        this.DragMove();
    }

    private void CloseBtn_Click(object sender, RoutedEventArgs e)
    {
      this.Hide();
      trayBar.ShowBalloonTip(2500, "QuickStartMenu", "Use o menu rápido para navegar.", Forms.ToolTipIcon.Info);
    }

    //private void Button_Click(object sender, RoutedEventArgs e)
    //{
    //  //Drawing.Image delete_icon = Drawing.Image.FromFile("delete_icon.png");
    //  //Drawing.Image details_icon = Drawing.Image.FromFile("details_icon.png");
    //  //Drawing.Image exclude_icon = Drawing.Image.FromFile("exclude_icon.png");
    //  //Drawing.Image favicon = Drawing.Image.FromFile("favicon.ico");
    //  //Drawing.Image open_file = Drawing.Image.FromFile("open_file.png");
    //  //Drawing.Image open_icon = Drawing.Image.FromFile("open_icon.png");
    //  using (MemoryStream ms = new MemoryStream())
    //  {
    //    favicon.Save(ms, favicon.RawFormat);
    //    byte[] details_img = ms.ToArray();
    //    string resp = Convert.ToBase64String(details_img);
    //  }
    //}
  }
}