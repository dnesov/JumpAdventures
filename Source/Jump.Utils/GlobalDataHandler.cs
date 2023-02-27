using Jump.Utils;

public class GlobalDataHandler
{
    public GlobalData Data => _data;

    public GlobalDataHandler()
    {
        _data = new GlobalData();
    }

    public void AcceptNotice()
    {
        Data.AcceptedNotice = true;
    }

    public void LoadData()
    {
        var serializer = new SaveDataSerializer();
        if (!serializer.SaveFileExists(_saveDataPath) || serializer.IsSaveFileEmpty(_saveDataPath))
        {
            SaveData();
            return;
        }

        _data = serializer.Deserialize(_saveDataPath);
    }

    public void SaveData()
    {
        var serializer = new SaveDataSerializer();
        serializer.Serialize(_data, _saveDataPath);
    }

    private GlobalData _data;
    private readonly string _saveDataPath = "user://global_save.yaml";
}