using AppBarMenu.Helpers;
using Entities.Entidades;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace AppBarMenu.Controllers
{
  public class FilesController
  {
    public FilesController()
    {
      _caminhoBase = FileHelper.GetCaminhoBase();
      AtualizaString();
    }
    private string _caminhoBase;
    private string _stringJsonLoad;
    public void AtualizaString()
    {
      try
      {
        string objetoJson = File.ReadAllText(_caminhoBase + "/Files.json");
        _stringJsonLoad = objetoJson;
      }
      catch (Exception ex)
      {
        if (string.IsNullOrEmpty(_stringJsonLoad))
        {
          File.WriteAllText(_caminhoBase + "/Files.json", JsonConvert.SerializeObject(new List<FileModel>()));
        }
      }

    }
    public void Remove(int index)
    {
      List<FileModel> list = GetFiles();
      list.RemoveAt(index);
      SerializeObj(list);
    }


    public void Add(FileModel model)
    {
      List<FileModel> list = GetFiles();
      list.Add(model);
      SerializeObj(list);
    }
    public void DeleteFile(int index)
    {
      List<FileModel> list = GetFiles();
      FileModel entity = list[index];
      Remove(index);
      File.Delete(entity.Path);
    }

    public void SerializeObj(List<FileModel> lista)
    {
      _stringJsonLoad = JsonConvert.SerializeObject(lista);
      File.WriteAllText(_caminhoBase + "/Files.json", _stringJsonLoad);
    }

    public List<FileModel> GetFiles()
    {
      try
      {
        List<FileModel> retorno = JsonConvert.DeserializeObject<List<FileModel>>(_stringJsonLoad);
        return retorno;
      }
      catch (Exception ex)
      {

        return new List<FileModel>();
      }
    }
  }
}
