using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectoryManager
{
  public class ImageManager
  {
    private const string IMAGEPATH = "\\~\\img";

    public ImageManager()
    {
      if (!Directory.Exists(IMAGEPATH))
        Directory.CreateDirectory(IMAGEPATH);
    }

    public string SaveImage(string pathCopy)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.FileName = pathCopy;
      string fileImage = Path.Combine(IMAGEPATH, string.Format("{1}_{0}", openFileDialog.SafeFileName, DateTime.Now.ToString("ddMMyyyyHHmmssms")));
      File.Copy(pathCopy, fileImage);
      return fileImage;
    }
  }
}
