using AppBarMenu.Infraestructure;
using AppBarMenu.Controllers;
using AppBarMenu.Helpers;
using Entities.Entidades;
using Microsoft.Win32;
using RegistryManager.RegManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;
using static RegistryManager.Enum.EnumRegHelper;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;
using System.Drawing;
using AppBarMenu.Entidades;
using System.Windows.Media;
using System.Runtime.Remoting.Contexts;
using System.Windows.Media.Imaging;
using System.Linq;

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
    private Drawing.Image favicon;
    private Drawing.Icon faviconIco;
    private Configuration _configuration;
    public MainWindow(Configuration config)
    {
      _configuration = config;
      InitializeComponent();
      InitialConfigs();
      if (_configuration["HideOnInit"])
        this.Hide();
    }

    //public static void ShowToast(string message, Guid itemId)
    //{
    //  ToastContentBuilder toast = new ToastContentBuilder();
    //  toast
    //  .AddArgument("item_Id", itemId.ToString())
    //  .AddText(message)
    //  .AddButton(new ToastButton()
    //  .SetContent("Desfazer")
    //  .AddArgument("action", "undo"))
    //  .Show();
    //  //ToastNotifier notifier = ToastNotificationManager.CreateToastNotifier();
    //  //ToastNotification notification = new ToastNotification(toast.GetXml());
    //  //notifier.Show(notification);
    //}

    //protected override void OnActivated(IActivatedEventArgs e)
    //{
    //  // Handle notification activation
    //  if (e is ToastNotificationActivatedEventArgs toastActivationArgs)
    //  {
    //    // Obtain the arguments from the notification
    //    ToastArguments args = ToastArguments.Parse(toastActivationArgs.Argument);

    //    // Obtain any user input (text boxes, menu selections) from the notification
    //    ValueSet userInput = toastActivationArgs.UserInput;

    //    // TODO: Show the corresponding content
    //  }
    //}

    private void _loadContextMenu()
    {
      ContextMenu contextMenu = new ContextMenu();
      MenuItem item1 = new MenuItem();
      item1.Header = "Ola";
      item1.Items.Add(new MenuItem()
      {
        Header = "Mundo"
      });
      contextMenu.Items.Add(item1);
      itemsListBox.ContextMenu = contextMenu;
      //List<CustomMenuItem> menuItems= new List<CustomMenuItem>();

      //CustomMenuItem menuItem = new CustomMenuItem();
      //menuItem.Title = "Teste";
      //menuItem.Icon = imagem_Delete;
      //menuItem.OnClick += ItemControlRenameFile_Click;
      //ListaDeItensBox.ContextMenu.Items.Add(menuItem);
    }

    private void InitialConfigs()
    {
      try
      {
        _controller = new FilesController(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
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
                imagem_Open = Drawing.Image.FromStream(msImg);

                break;
              case 3:
                imagem_OpenFile = Drawing.Image.FromStream(msImg);
                break;
              case 4:
                imagem_Exclude = Drawing.Image.FromStream(msImg);
                break;
              case 5:
                faviconIco = new Drawing.Icon(msImg);
                favicon = faviconIco.ToBitmap();
                break;
              default:
                break;
            }
          }
        }
        _reloadList();
        //_loadContextMenu();
        itemsListBox.AllowDrop = true;
        trayBar.Icon = faviconIco;
        trayBar.Visible = true;
        trayBar.DoubleClick += DoubleClickTrayBar;
        string local = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        //iniciaVerificacaoUpdate(local);
        try
        {
          bool isInitializeWith = _configuration["StartWithSystem"];

          if (isInitializeWith)
            RegistryHelper.RegInitializeWithWin(ToDo.Create);
          else
            RegistryHelper.RegInitializeWithWin(ToDo.Delete);

          RegistryHelper.RegContextWindowsMenu(ToDo.Create, local + @"\QuickStartMenu.exe", "Adicionar ao Quickly Menu");
        }
        catch (Exception ex) { }
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

    //FileSystemWatcher fsw;

    //void iniciaVerificacaoUpdate(string local)
    //{
    //  Thread trheadFSW = new Thread(() =>
    //  {
    //    fsw = new FileSystemWatcher(local);
    //    fsw.EnableRaisingEvents = true;
    //    fsw.IncludeSubdirectories = true;
    //    fsw.Changed += new FileSystemEventHandler(UpdatedContext);
    //    fsw.Filter = "Files.json";
    //  });
    //  trheadFSW.Start();
    //}

    //private void UpdatedContext(object sender, FileSystemEventArgs e)
    //{
    //  Action updateList = () => { _reloadList(); };
    //  this.Dispatcher.Invoke(updateList);
    //}

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
        if (!_listModels[itemsListBox.SelectedIndex].IsGroup)
          System.Diagnostics.Process.Start(_listModels[itemsListBox.SelectedIndex].Path);
        else
        {
          FileModel group = _listModels[itemsListBox.SelectedIndex];
          if (group.ListChildren != null)
            group.ListChildren.ForEach(x => System.Diagnostics.Process.Start(x.Path));
        }
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
        FileModel item = _listModels[objValueItemIndex];
        System.Diagnostics.Process.Start("explorer.exe", item.Path.Replace(item.Name, string.Empty));
      }
      catch (Exception ex) { }
    }

    public void RefrashReoloadList()
    {
      _reloadList();
    }


    void _reloadList()
    {
      _controller.RefreshString();
      _listModels = _controller.GetFiles();
      itemsListBox.Items.Clear();
      
      JumpList jumpList = new JumpList();
      foreach (FileModel item in _listModels)
      {
        itemsListBox.Items.Add(item);
      }
      foreach (FileModel item in _listModels.Where(x => !x.IsGroup))
      {
        string name = string.IsNullOrEmpty(item.Extension) ? item.Name : item.Name.Replace(item.Extension, string.Empty);
        JumpTask jumpTask = new JumpTask();
        jumpTask.ApplicationPath = item.Path;
        jumpTask.IconResourcePath = string.IsNullOrEmpty(item.ImagePath) ? item.Path : item.ImagePath;
        jumpTask.IconResourceIndex = 1;
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
          if (string.IsNullOrEmpty(item.ImagePath))
          {
            if (!string.IsNullOrEmpty(item.Extension))
              imagemIcon = Drawing.Icon.ExtractAssociatedIcon(item.Path).ToBitmap();
            else
              imagemIcon = Drawing.Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Environment.SystemDirectory, "..\\explorer.exe")).ToBitmap();
          }
          else
          {
            byte[] imageBytes = File.ReadAllBytes(item.ImagePath);
            Drawing.Image img = null;
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
              img = Drawing.Image.FromStream(ms);
            }

            imagemIcon = img;
          }
        }
        catch (Exception ex)
        {
          imagemIcon = favicon;

        }
        //string.IsNullOrEmpty(item.Extension) ? item.Name
        //  : item.Name.Replace(item.Extension, string.Empty), imagemIcon, ClickTraybar
        trayBar.ContextMenuStrip.Items.Add(
          new Forms.ToolStripDropDownButton(
            string.IsNullOrEmpty(item.Extension) ? item.Name
          : item.Name.Replace(item.Extension, string.Empty), imagemIcon
          , new Forms.ToolStripButton($"{i}. Abrir", imagem_Open, ClickTraybarOpen)
          , new Forms.ToolStripButton($"{i}. Abrir Pasta", imagem_OpenFile, ClickTraybarOpenFolder)
          , new Forms.ToolStripButton($"{i}. Renomear", imagem_Open, RenameFileByTraybar)));
      }
      trayBar.ContextMenuStrip.Items.Add(new
      Forms.ToolStripButton("Sair", imagem_Exclude, TrayBarCloseProgram));
    }

    void TrayBarCloseProgram(object sender, EventArgs e)
    {
      Environment.Exit(0);
    }

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
        _reloadList();
      }
    }

    private void ChangeImageFile(int? item = null)
    {
      int selectedIndex = item ?? itemsListBox.SelectedIndex;
      CustomImageDialog changePage = new CustomImageDialog(
        string.IsNullOrEmpty(_controller.GetFiles()[selectedIndex].ImagePath)
        ? _controller.GetFiles()[selectedIndex].Path
        : _controller.GetFiles()[selectedIndex].ImagePath);
      changePage.OnDone += (newName, context) =>
      {
        CustomImageDialog c = (CustomImageDialog)context;
        _controller.ChangeImage(selectedIndex, newName);
        _reloadList();
      };

      changePage.OnFinish += (context) =>
      {
        CustomImageDialog c = (CustomImageDialog)context;
        c.Close();
      };

      changePage.Show();
    }

    private void RenameFile(int? item = null)
    {
      int selectedIndex = item ?? itemsListBox.SelectedIndex;
      CustomDialog changePage = new CustomDialog("Concluír", "Digite o nome:", _controller.GetFiles()[selectedIndex].Name);
      changePage.OnDone += (newName, context) =>
      {
        CustomDialog c = (CustomDialog)context;
        _controller.ChangeNameItem(selectedIndex, newName);
        _reloadList();
      };

      changePage.OnFinish += (context) =>
      {
        CustomDialog c = (CustomDialog)context;
        c.Close();
      };

      changePage.Show();
    }

    private void RenameFileByTraybar(object sender, EventArgs e)
    {
      Forms.ToolStripButton entidade = sender as Forms.ToolStripButton;
      string[] splittedValueName = entidade.AccessibilityObject.Name.Split('.');
      int objValueItemIndex = Int32.Parse(splittedValueName[0]);
      RenameFile(objValueItemIndex);
    }

    private void ItemControlRenameFile_Click(object sender, RoutedEventArgs e)
    {
      RenameFile();
    }

    private void ItemControlChangeImageFile_Click(object sender, RoutedEventArgs e)
    {
      ChangeImageFile();
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
        _reloadList();
      }

    }

    private void Box_DragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
    }

    private void Box_DragOver(object sender, DragEventArgs e)
    {
    }

    private void Box_Drop(object sender, DragEventArgs e)
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
      _reloadList();
    }

    private void ItemControlExclude_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (itemsListBox.SelectedIndex != -1)
        {
          var result = MessageBox.Show("Ao confirmar, o arquivo será apagado do seu sistema de arquivos, deseja continuar?",
        "Confirmar?", MessageBoxButton.YesNo, MessageBoxImage.Question);
          if (result == MessageBoxResult.Yes)
          {
            _controller.DeleteFile(itemsListBox.SelectedIndex);
            _reloadList();
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
        if (itemsListBox.SelectedIndex != -1)
        {
          if(!_listModels[itemsListBox.SelectedIndex].IsGroup)
            System.Diagnostics.Process.Start(_listModels[itemsListBox.SelectedIndex].Path);
          else
          {
            FileModel group = _listModels[itemsListBox.SelectedIndex];
            if (group.ListChildren != null)
              group.ListChildren.ForEach(x => System.Diagnostics.Process.Start(x.Path));
          }


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
        if (itemsListBox.SelectedIndex != -1)
        {
          FileModel item = _listModels[itemsListBox.SelectedIndex];
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
        if (itemsListBox.SelectedIndex != -1)
        {
          int itemSelecionado = itemsListBox.SelectedIndex;
          _controller.Remove(itemSelecionado);
          _reloadList();
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
        if (itemsListBox.SelectedIndex != -1)
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

    private void AddNewGroup(object sender, EventArgs e)
    {
      FileModel fileModel = new FileModel();
      CustomDialog customDialog = new CustomDialog("Próximo", "Novo Grupo:", "");
      customDialog.Show();
      customDialog.OnFinish += (context) =>
      {
        CustomDialog customDialog1 = context as CustomDialog;
        customDialog1.Close();
      };

      customDialog.OnDone += (newName, context) =>
      {
        string nameGroup = "";
        nameGroup = newName;

        CustomImageDialog customImageDialog = new CustomImageDialog("QuickStartMenu.exe");
        customImageDialog.Show();
        customImageDialog.OnFinish += (context1) =>
        {
          CustomImageDialog context2 = context1 as CustomImageDialog;
          context2.Close();
        };

        customImageDialog.OnDone += (newName1, context1) =>
        {
          _controller.AddGroup(newName, newName1, null);
          _reloadList();
        };
      };


    }


    private List<MenuItem> getMenuDefaultItem()
    {
      List<MenuItem> menuItems = new List<MenuItem>();
      MenuItem openGroup = new MenuItem()
      {
        Header = "Abrir",
      };
      openGroup.Click += ItemControlOpen_Click;
      menuItems.Add(openGroup);

      MenuItem removeGroup = new MenuItem()
      {
        Header = "Remover",
      };
      removeGroup.Click += ItemControlRemove_Click;
      menuItems.Add(removeGroup);

      MenuItem renameGroup = new MenuItem()
      {
        Header = "Renomear",
      };
      removeGroup.Click += ItemControlRemove_Click;
      menuItems.Add(removeGroup);


      return menuItems;
    }

    private List<MenuItem> getMenuDefaultGrupo()
    {
      List<MenuItem> menuItems = new List<MenuItem>();
      MenuItem openGroup = new MenuItem()
      {
        Header = "Abrir",
      };
      openGroup.Click += ItemControlOpen_Click;
      menuItems.Add(openGroup);

      MenuItem removeGroup = new MenuItem()
      {
        Header = "Remover",
      };
      removeGroup.Click += ItemControlRemove_Click;
      menuItems.Add(removeGroup);

      return menuItems;
    }

    private void ListaDeItensBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        if (itemsListBox.SelectedIndex != -1)
        {
          ContextMenu contextMenu = new ContextMenu();
          FileModel file = _controller.GetFiles()[itemsListBox.SelectedIndex];

          if (file.IsGroup)
          {
            getMenuDefaultGrupo().ForEach(x => contextMenu.Items.Add(x));
          }
          else
          {
            getMenuDefaultItem().ForEach(x => contextMenu.Items.Add(x));
          }
          itemsListBox.ContextMenu = contextMenu;
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

    Thickness defaultMarginValue;
    double defaulWidthValue;
    System.Windows.Controls.Image groupImage;
    System.Windows.Controls.Label groupLabel;
    System.Windows.Controls.Button groupExitButton;

    private void loadContext()
    {
      FileModel file = _controller.Context;
      defaulWidthValue = itemsListBox.Width;
      defaultMarginValue = itemsListBox.Margin;
      itemsListBox.Width = 420;
      itemsListBox.Margin = new Thickness(180, 85, 0, 0);
      groupImage = new System.Windows.Controls.Image();
      groupImage.Height = 100;
      groupImage.Width = 100;
      groupImage.Margin = new Thickness(18, 85, 0, 0);
      groupImage.VerticalAlignment = VerticalAlignment.Top;
      groupImage.HorizontalAlignment = HorizontalAlignment.Left;
      if (!file.ImagePath.Equals("QuickStartMenu.exe"))
      {
        BitmapImage imageBit = new BitmapImage(new Uri(file.ImagePath));
        groupImage.Source = imageBit;
      }
        
      


      groupLabel = new System.Windows.Controls.Label();
      groupLabel.Margin = new Thickness(18, 196, 0, 0);
      groupLabel.Content = file.Name;
      groupLabel.Width = 72;
      groupLabel.VerticalAlignment = VerticalAlignment.Top;
      groupLabel.HorizontalAlignment = HorizontalAlignment.Left;
      groupLabel.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

      groupExitButton = new System.Windows.Controls.Button();
      groupExitButton.Margin = new Thickness(104, 196, 0, 0);
      groupExitButton.Content = "Voltar";
      groupExitButton.Width= 70;
      groupExitButton.VerticalAlignment= VerticalAlignment.Top;
      groupExitButton.HorizontalAlignment = HorizontalAlignment.Left;
      groupExitButton.Click += (sender, e) =>
      {
        unloadContext();
      };
      groupExitButton.Visibility = Visibility.Visible;
      groupImage.Visibility = Visibility.Visible;
      groupLabel.Visibility = Visibility.Visible;
      BtnAdc.Visibility = Visibility.Hidden;
      MainGrid.Children.Add(groupExitButton);
      MainGrid.Children.Add(groupLabel);
      MainGrid.Children.Add(groupImage);
    }

    private void unloadContext()
    {
      itemsListBox.Width = defaulWidthValue;
      itemsListBox.Margin = defaultMarginValue;
      MainGrid.Children.Remove(groupExitButton);
      MainGrid.Children.Remove(groupLabel);
      MainGrid.Children.Remove(groupImage);
      BtnAdc.Visibility= Visibility.Visible;
      _controller.ClearContext();
      _reloadList();
    }

    private void ListaDeItensBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      ListBox target = sender as ListBox;
      FileModel file = target.SelectedItem as FileModel;
      if (file != null && file.IsGroup)
      {
        _controller.ChangeContext(file);
        loadContext();
        _reloadList();
      }
    }

    //private void itemsListBox_MouseUp(object sender, MouseButtonEventArgs e)
    //{
    //  if (_controller.IsContextualized)
    //  {
    //    _controlzler.ClearContext();
    //    itemsListBox.Width = defaulWidthValue;
    //    itemsListBox.Margin = defaultMarginValue;
    //  }
    //}

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

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      //ShowToast("Teste", Guid.NewGuid());
    }


  }
}