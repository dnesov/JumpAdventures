using Jump.Utils.SaveData;

public class GlobalDataHandler
{
    public ConfigSaveData Data => _data;

    public GlobalDataHandler()
    {
        _data = new ConfigSaveData();
    }

    public void LoadData()
    {
        var serializer = new SaveDataSerializer();
        if (!serializer.SaveFileExists(_saveDataPath) || serializer.IsSaveFileEmpty(_saveDataPath))
        {
            SaveData();
            return;
        }

        _data = serializer.DeserializeYaml<ConfigSaveData>(_saveDataPath);
    }

    public void SaveData()
    {
        var serializer = new SaveDataSerializer();
        serializer.SerializeYaml(_data);
    }

    private ConfigSaveData _data;
    private readonly string _saveDataPath = "user://config.yaml";
}