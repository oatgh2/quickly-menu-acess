using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryManager.Enum
{
  public static class EnumRegHelper
  {
    public enum ToDo
    {
      [Description("Create")]
      Create = 1,
      Delete = 2
    }
  }
}
