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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

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

    public MainWindow()
    {
      InitializeComponent();
      InitialConfigs();
    }


    private void InitialConfigs()
    {
      try
      {
        _controller = new FilesController();
        ReloadList();
        ListaDeItensBox.AllowDrop = true;
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

    void ReloadList()
    {
      _listModels = _controller.GetFiles();
      ListaDeItensBox.Items.Clear();
      JumpList jumpList = new JumpList();
      foreach (FileModel item in _listModels)
      {
        ListaDeItensBox.Items.Add(item.Name);
        JumpTask jumpTask = new JumpTask();
        jumpTask.ApplicationPath = item.Path;
        jumpTask.IconResourcePath = item.Path;
        jumpTask.Title = item.Name;
        jumpTask.Description = "Abrir " + item.Name;
        jumpTask.CustomCategory = "Adcionados TOP(10)";
        jumpList.JumpItems.Add(jumpTask);
      }
      JumpList.SetJumpList(App.Current, jumpList);
    }

    //private void BtnAdd2_Click(object sender, RoutedEventArgs e)
    //{
    //  MessageBox.Show("clicado no xmlamammsasaqs");
    //}

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog fileDialog = new OpenFileDialog();
      fileDialog.ShowDialog();
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
      var result = MessageBox.Show("Ao confirmar, o arquivo será apagado do seu sistema de arquivos, deseja continuar?",
        "Confirmar?", MessageBoxButton.YesNo, MessageBoxImage.Question);
      if(result == MessageBoxResult.Yes)
      {
       _controller.DeleteFile(ListaDeItensBox.SelectedIndex);
        ReloadList();
      }
    }

    private void ItemControlOpen_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        System.Diagnostics.Process.Start(_listModels[ListaDeItensBox.SelectedIndex].Path);
      }
      catch (Exception ex) { }
      
    }
    private void ItemControlOpenFile_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var item = _listModels[ListaDeItensBox.SelectedIndex];
        System.Diagnostics.Process.Start("explorer.exe", item.Path.Replace(item.Name, string.Empty));
      }
      catch (Exception ex) { }
    }


    private void ItemControlRemove_Click(object sender, RoutedEventArgs e)
    {
      int itemSelecionado = ListaDeItensBox.SelectedIndex;
      _controller.Remove(itemSelecionado);
      ReloadList();
    }


    private void ItemControlDetails_Click(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("ClicadoDetails");
    }
    private void ListaDeItensBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      if(ListaDeItensBox.SelectedItems.Count > 0)
      {
        ContextStripListOpen.IsOpen = true;
      }
    }
  }
}