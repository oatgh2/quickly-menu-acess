using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppBarMenu
{
  /// <summary>
  /// Lógica interna para CustomImageDialog.xaml
  /// </summary>
  public partial class CustomImageDialog : Window
  {
    private OpenFileDialog _imageDialog = new OpenFileDialog();
    public CustomImageDialog(string _pathImage)
    {
      InitializeComponent();
      Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(_pathImage);
      BitmapImage initialImg = new BitmapImage();
      Bitmap img = icon.ToBitmap();

      try
      {
        using (MemoryStream ms = new MemoryStream())
        {
          img.Save(ms, ImageFormat.Png);
          ms.Position = 0;

          initialImg.BeginInit();
          initialImg.StreamSource = ms;
          initialImg.CacheOption= BitmapCacheOption.OnLoad;
          initialImg.EndInit();
          initialImg.Freeze();
        }
      }
      catch (Exception ex)
      {
      }
      ImagePreview.Source = initialImg;
    }

    public delegate void NameChanged(string newName, object context);
    public delegate void Finishing(object context);

    public event NameChanged OnDone;
    public event Finishing OnFinish;

    public void done()
    {
      string typedValue = _imageDialog.FileName;
      if (string.IsNullOrEmpty(typedValue))
        MessageBox.Show("A imagem não pode ser vazia");
      else
      {
        if (OnDone != null)
        {
          OnDone.Invoke(typedValue, this);
          if (OnFinish != null)
            OnFinish.Invoke(this);
        }
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      done();
    }

    private void ImagePreview_MouseUp(object sender, MouseButtonEventArgs e)
    {
      OpenFileDialog fileDialog = new OpenFileDialog();
      fileDialog.Multiselect = false;
      fileDialog.Title = "Selecione uma imagem";
      fileDialog.Filter = "Arquivos de imagem |*.png; *.jpg; *.ico";
      fileDialog.ShowDialog();
      if (!string.IsNullOrEmpty(fileDialog.FileName))
      {
        byte[] image = File.ReadAllBytes(fileDialog.FileName);
        BitmapImage img = new BitmapImage();
        using (MemoryStream ms = new MemoryStream(image))
        {
          img.StreamSource = ms;
        }
        ImagePreview.Source = img;
      }
    }
  }
}
