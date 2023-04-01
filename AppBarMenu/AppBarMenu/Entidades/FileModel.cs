using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace Entities.Entidades
{
  public class FileModel
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    public string MimmeType { get; set; }
    public string ImagePath { get; set; }
    public byte[] Image
    {
      get
      {
        byte[] file = null;
        
        if (!File.Exists(ImagePath))
        {
          using (MemoryStream ms = new MemoryStream())
          {
            Bitmap icon = Icon.ExtractAssociatedIcon(Path).ToBitmap();
            icon.Save(ms, ImageFormat.Png);
            file = ms.ToArray();
            
          }
        }
        else
        {
          file = File.ReadAllBytes(ImagePath);
        }
        return file;
      }
    }
  }
}
