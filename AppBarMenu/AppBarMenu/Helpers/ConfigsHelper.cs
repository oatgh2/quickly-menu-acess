using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBarMenu.Helpers
{
  public class Configuration
  {
    string _path = "./config.json";
    FileSystemWatcher _fsw;


    public dynamic this[string key]
    {
      get
      {
        if (_dataDictonary.ContainsKey(key))
          return _dataDictonary[key];
        else
          return "";
      }
      set
      {
        _dataDictonary[key] = value;
        _readedJson = JsonConvert.SerializeObject(_dataDictonary);
        File.WriteAllText(_path, _readedJson);
      }
    }

    private string _readedJson = "";

    private Dictionary<string, dynamic> _dataDictonary = new Dictionary<string, dynamic>();

    private void verifyKeys()
    {
      if (!_dataDictonary.ContainsKey("StartWithSystem"))
        _dataDictonary.Add("StartWithSystem", true);

      if (!_dataDictonary.ContainsKey("OpenWhenAddByContextMenu"))
        _dataDictonary.Add("OpenWhenAddByContextMenu", true);

      if (!_dataDictonary.ContainsKey("InitializeInterop"))
        _dataDictonary.Add("InitializeInterop", true);

      if (!_dataDictonary.ContainsKey("HideOnInit"))
        _dataDictonary.Add("HideOnInit", true);

      _readedJson = JsonConvert.SerializeObject(_dataDictonary);
      File.WriteAllText(_path, _readedJson);
    }

    private void load()
    {

      if (File.Exists(_path))
      {
        string stringfyedJson = File.ReadAllText(_path);
        _readedJson = stringfyedJson;
        Dictionary<string, dynamic> json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(stringfyedJson);
        _dataDictonary = json;
        verifyKeys();
      }
      else
      {
        verifyKeys();
      }
    }

    private void launch()
    {
      _fsw = new FileSystemWatcher("./");
      _fsw.Filter = _path.Split('/')[1];
      _fsw.EnableRaisingEvents = true;
      _fsw.Changed += new FileSystemEventHandler(UpdatedConfigs);
    }

    private void UpdatedConfigs(object sender, FileSystemEventArgs e)
    {
      string stringfyedJson = File.ReadAllText(_path);
      _readedJson = stringfyedJson;
      Dictionary<string, dynamic> json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(_readedJson);
      _dataDictonary = json;
    }

    public void Initialize()
    {
      load();
      launch();
    }

  }
}
