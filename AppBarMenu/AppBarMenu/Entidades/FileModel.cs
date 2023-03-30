using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entidades
{
  public class FileModel
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    public string MimmeType { get; set; }
    public string ImagePath { get; set;}
  }
}
