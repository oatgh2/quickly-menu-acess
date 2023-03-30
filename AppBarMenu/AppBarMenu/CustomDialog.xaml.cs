using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppBarMenu
{
  /// <summary>
  /// Lógica interna para ChangeNameFile.xaml
  /// </summary>
  public partial class CustomDialog : Window
  {
    public delegate void NameChanged(string newName, object context);
    public delegate void Finishing(object context);

    public event NameChanged OnDone;
    public event Finishing OnFinish;
    public CustomDialog(string buttonName, string title = "", string defaultValue = "")
    {
      InitializeComponent();
      this.Title = title;
      this.BtnClickSaveNewName.Content = buttonName;
      this.NewNameField.Text = defaultValue;
    }

    public void done()
    {
      string typedValue = NewNameField.Text;
      if (string.IsNullOrEmpty(typedValue))
        MessageBox.Show("O novo nome não pode ser vazio");
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

    private void BtnClickSaveNewName_Click(object sender, RoutedEventArgs e)
    {
      done();
    }

    private void NewNameField_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
        done();
    }
  }
}
