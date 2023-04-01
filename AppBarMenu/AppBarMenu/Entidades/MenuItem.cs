using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AppBarMenu.Entidades
{
  public class CustomMenuItem
  {
    public delegate void Click(Object sender, RoutedEventArgs e);
    public string Title { get; set; }
    public object Icon { get; set; }
    public event Click OnClick;
  }
}
