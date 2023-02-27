using System;
using Godot;
using YamlDotNet.Serialization;

namespace Jump.Utils
{
    public class SaveDataSerializer
    {
        public void Serialize(GlobalData data, string path)
        {
            var file = new File();
            file.Open(path, File.ModeFlags.Write);

            var serializer = new Serializer();

            file.StoreString(serializer.Serialize(data));
            file.Close();
        }

        public bool IsSaveFileEmpty(string path)
        {
            var file = new File();
            file.Open(path, File.ModeFlags.Read);
            var len = file.GetLen() == 0;
            return len;
        }

        public GlobalData Deserialize(string from)
        {
            var file = new File();
            file.Open(from, File.ModeFlags.Read);

            var deserializer = new DeserializerBuilder().Build();
            GlobalData saveData = new GlobalData();

            try
            {
                saveData = deserializer.Deserialize<GlobalData>(file.GetAsText());
            }
            catch (Exception e)
            {
                OS.Alert($"There was an error reading global save data. Settings and essence will be reset.\n {e.Message}", "Failed to read global save data!");
            }

            file.Close();
            return saveData;
        }

        public bool SaveFileExists(string at)
        {
            var file = new File();
            var exists = file.FileExists(at);
            return exists;
        }
    }
}