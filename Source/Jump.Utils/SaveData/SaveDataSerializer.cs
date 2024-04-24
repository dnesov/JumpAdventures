using System;
using Ceras;
using Godot;
using YamlDotNet.Serialization;

namespace Jump.Utils.SaveData
{
    public class SaveDataSerializer
    {
        public SaveDataSerializer()
        {
            _serializerConfig.VersionTolerance.Mode = VersionToleranceMode.Standard;
        }
        public void SerializeYaml(SaveDataBase data)
        {
            var path = $"user://{data.FileName}";
            var file = new File();
            file.Open(path, File.ModeFlags.Write);

            var serializer = new Serializer();

            file.StoreString(serializer.Serialize(data));
            file.Close();
        }

        public void SerializeCeras<T>(T data) where T : SaveDataBase, new()
        {
            var path = $"user://{data.FileName}";
            var file = new File();
            Error error = file.Open(path, File.ModeFlags.Write);

            if (error != Error.Ok)
                throw new System.IO.FileNotFoundException($"Cannot write save data at {path}!");

            _cerasSaveBuffer = new byte[12];
            var serializer = new CerasSerializer(_serializerConfig);
            serializer.Serialize(data, ref _cerasSaveBuffer);
            file.StoreBuffer(_cerasSaveBuffer);
            file.Close();
        }

        public bool IsSaveFileEmpty(string path)
        {
            var file = new File();
            file.Open(path, File.ModeFlags.Read);
            var len = file.GetLen() == 0;
            return len;
        }

        public T DeserializeYaml<T>(string fromPath) where T : SaveDataBase, new()
        {
            var file = new File();
            file.Open(fromPath, File.ModeFlags.Read);

            var deserializer = new DeserializerBuilder().Build();
            T saveData = new T();

            try
            {
                saveData = deserializer.Deserialize<T>(file.GetAsText());
            }
            catch (Exception e)
            {
                OS.Alert($"There was an error reading save data.\n {e.Message}", "Failed to read save data!");
            }

            file.Close();
            return saveData;
        }

        public T DeserializeCeras<T>(string fromPath, bool createIfMissing = true) where T : SaveDataBase, new()
        {
            var path = $"user://{fromPath}";
            var file = new File();

            var exists = SaveFileExists(path);
            if (!exists)
            {
                if (createIfMissing) CreateMissingSave(path);
            }

            Error error = file.Open(path, File.ModeFlags.Read);

            if (error != Error.Ok)
                throw new System.IO.FileNotFoundException($"Cannot read save data at {path}!");

            var length = (long)file.GetLen();
            _cerasSaveBuffer = file.GetBuffer(length);
            file.Close();
            var deserializer = new CerasSerializer(_serializerConfig);

            T data = new T();

            if (_cerasSaveBuffer.Length > 0)
            {
                try
                {
                    data = deserializer.Deserialize<T>(_cerasSaveBuffer);
                }
                catch
                {
                    OS.Alert("It seems like you're playing with an older version of the save file. All progress will be reset.", "Older save file format detected!");
                }
            }
            return data;
        }

        public bool SaveFileExists(string at)
        {
            var file = new File();
            var exists = file.FileExists(at);
            return exists;
        }

        private void CreateMissingSave(string at)
        {
            var file = new File();
            file.Open(at, File.ModeFlags.Write);
            file.Close();
        }

        private byte[] _cerasSaveBuffer;
        private SerializerConfig _serializerConfig = new SerializerConfig();
    }
}