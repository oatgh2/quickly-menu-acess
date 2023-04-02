using AppBarMenu.Helpers;
using Entities.Entidades;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using DirectoryManager;

namespace AppBarMenu.Controllers
{

  public class FilesController
  {
    public FilesController(string workingDirectory)
    {
      _caminhoBase = FileHelper.GetCaminhoBase();
      _imageManager = new ImageManager();
      RefreshString();
    }
    public FilesController()
    {
      _caminhoBase = FileHelper.GetCaminhoBase();
      _imageManager = new ImageManager();
      RefreshString();
    }
    private string _caminhoBase;
    private string _stringJsonLoad;
    private ImageManager _imageManager;


    private FileModel _context;

    public FileModel Context { get { return _context; } }

    public bool IsContextualized
    {
      get
      {
        return Context != null;
      }
    }

    public string ChangeContext(FileModel fileModel)
    {
      _context = fileModel;
      return fileModel.Name;
    }

    public void ClearContext()
    {
      _context = null;
    }

    public void ChangeImage(int itemPosition, string newImagePath)
    {
      if (string.IsNullOrEmpty(newImagePath))
        return;

      List<FileModel> files = GetFiles();

      FileModel file = files[itemPosition];

      if (file != null)
      {
        files.Remove(file);
        file.ImagePath = _imageManager.SaveImage(newImagePath);
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
        string jsonObj = File.ReadAllText(_caminhoBase + "/Files.json");
        _stringJsonLoad = jsonObj;
      }
      catch (Exception ex)
      {
        if (string.IsNullOrEmpty(_stringJsonLoad))
        {
          File.WriteAllText(_caminhoBase + "/Files.json", JsonConvert.SerializeObject(new List<FileModel>()));
        }
      }

    }

    public void AddGroup(string nameGroup, string imagePath, List<FileModel> listItens)
    {
      FileModel file = new FileModel();
      file.Name = nameGroup;
      file.ImagePath = imagePath;
      file.ListChildren = listItens;
      file.IsGroup = true;
      file.Id = Guid.NewGuid();
      List<FileModel> files = GetFiles();
      files.Add(file);
      SerializeObj(files);
    }

    public void AddInGroup(Guid idGroup, FileModel fileModel)
    {
      List<FileModel> files = GetFiles();

      FileModel group = files.Where(x => x.Id == idGroup).FirstOrDefault();


      fileModel.IdGroup = idGroup;
      files.Add(fileModel);
      if (group != null)
      {
        group.ListChildren.Add(fileModel);
      }
      SerializeObj(files);
    }

    public void AddInGroup(Guid idGroup, List<FileModel> filesModel)
    {
      List<FileModel> files = GetFiles();

      FileModel group = files.Where(x => x.Id == idGroup).FirstOrDefault();

      foreach (FileModel item in filesModel)
      {
        item.IdGroup = idGroup;
        files.Add(item);
      }

      if (group != null)
      {
        group.ListChildren.AddRange(filesModel);
      }
      SerializeObj(files);
    }

    public void RemoveInGroup(Guid idGroup, Guid idFile)
    {
      List<FileModel> files = GetFiles();
      FileModel group = files.Where(x => x.Id == idGroup).FirstOrDefault();
      FileModel removedItem = group.ListChildren.Where(x => x.Id == idFile).FirstOrDefault();
      group.ListChildren.Remove(removedItem);
      files.Remove(group);
      files.Add(group);
      removedItem.IdGroup = null;
      files.Remove(removedItem);
      files.Add(removedItem);
      SerializeObj(files);
    }

    private void SaveContext()
    {
      FileModel fileContext = Context;
      _context = null;
      List<FileModel> files = GetFiles();
      files.Remove(files.Where(x => x.Id == fileContext.Id).FirstOrDefault());
      files.Add(fileContext);
      SerializeObj(files);
      _context = fileContext;
    }

    public void MoveOutGroup(int indexFile)
    {
      List<FileModel> files = GetFiles();
      FileModel fileMoving = files[indexFile];
      files.Remove(fileMoving);
      Context.ListChildren = files;
      FileModel actuallyContext = Context;
      _context = null;
      Add(fileMoving);
      _context = actuallyContext;
    }

    public void Remove(int index)
    {
      List<FileModel> list = GetFiles();

      if (Context != null)
      {
        list.RemoveAt(index);
        Context.ListChildren = list;
        SaveContext();
      }
      else
      {
        list.RemoveAt(index);
        SerializeObj(list);
      }
    }



    public void Add(FileModel model)
    {
      List<FileModel> list = GetFiles();
      if (Context != null)
      {
        list.Add(model);
        Context.ListChildren = list;
        SaveContext();
      }
      else
      {
        list.Add(model);
        SerializeObj(list);
      }
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
        if (Context == null)
        {
          List<FileModel> retorno = JsonConvert.DeserializeObject<List<FileModel>>(_stringJsonLoad);
          return retorno;
        }
        else
        {
          if (Context.ListChildren == null)
            return new List<FileModel>();

          return Context.ListChildren;
        }
      }
      catch (Exception ex)
      {

        return new List<FileModel>();
      }
    }
  }
}
