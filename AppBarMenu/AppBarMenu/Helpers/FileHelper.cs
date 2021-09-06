using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBarMenu.Helpers
{
  public static class FileHelper
  {
    public static string GetCaminhoBase()
    {
      return AppDomain.CurrentDomain.BaseDirectory.ToString();
    }
  }
}
