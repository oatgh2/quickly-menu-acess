using AppBarMenu.Helpers;
using Entities.Entidades;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace AppBarMenu.Controllers
{
  public class FilesController
  {
    public FilesController()
    {
      _caminhoBase = FileHelper.GetCaminhoBase();
      RefreshString();
    }
    private string _caminhoBase;
    private string _stringJsonLoad;

    public void ChangeImage(int itemPosition, string newImagePath)
    {
      if (string.IsNullOrEmpty(newImagePath))
        return;

      List<FileModel> files = GetFiles();

      FileModel file = files[itemPosition];

      if (file != null)
      {
        files.Remove(file);
        file.ImagePath = newImagePath;
        files.Add(file);
        SerializeObj(files);
      }
      else
      {
        return;
      }
    }

    public void ChangeNameItem(int itemPosition, string newName)
    {
      if (string.IsNullOrEmpty(newName))
        return;

      List<FileModel> files = GetFiles();

      FileModel file = files[itemPosition];

      if (file != null)
      {
        files.Remove(file);
        file.Name = newName;
        files.Add(file);
        SerializeObj(files);
      }
      else
      {
        return;
      }
    }


    public void RefreshString()
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
      model.ImagePath = model.Path;
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
