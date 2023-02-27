using System.Collections.Generic;
using System.IO;
using Ceras;
using Godot;
using Directory = Godot.Directory;
using File = Godot.File;

namespace Jump.Utils
{
    /// <summary>
    /// Handles the serialization and deserialization of worldpack savedata, as well as general operations on it, such as caching.
    /// </summary>
    public class WorldSaveDataHandler
    {
        public WorldSaveDataHandler()
        {
            _cachedSaveData = new Dictionary<string, WorldSaveData>();
        }

        public WorldSaveData LoadData(string worldId)
        {
            var file = new File();
            string path = $"{SAVE_FOLDER_PATH}{worldId}{EXTENSION}";
            Error error = file.Open(path, File.ModeFlags.Read);

            _logger.Info($"Loading world save file at: {path}.");

            if (error != Error.Ok)
                throw new FileNotFoundException($"Cannot read world save data at {path}!");

            var length = (long)file.GetLen();
            _saveDataBuffer = file.GetBuffer(length);
            file.Close();
            var deserializer = new CerasSerializer();
            var data = deserializer.Deserialize<WorldSaveData>(_saveDataBuffer);

            CacheSaveData(worldId, data);

            return data;
        }

        public WorldSaveData TryLoadData(string worldId)
        {
            if (IsCached(worldId)) return LoadCached(worldId);
            if (!SaveDataExists(worldId)) return CreateNewSave(worldId);
            return LoadData(worldId);
        }

        public bool TrySaveData(string forWorldId, WorldSaveData saveData)
        {
            CreateSaveFolder();
            var file = new File();
            string path = $"{SAVE_FOLDER_PATH}{forWorldId}{EXTENSION}";
            Error error = file.Open(path, File.ModeFlags.Write);

            if (error != Error.Ok)
                throw new FileNotFoundException($"Cannot write world save data at {path}!");

            var serializer = new CerasSerializer();
            serializer.Serialize<WorldSaveData>(saveData, ref _saveDataBuffer);

            file.StoreBuffer(_saveDataBuffer);
            file.Close();

            return true;
        }

        public WorldSaveData CreateNewSave(string forWorldId)
        {
            var saveData = new WorldSaveData();
            TrySaveData(forWorldId, saveData);
            return saveData;
        }

        public bool CreateSaveFolder()
        {
            var dir = new Directory();
            dir.Open("user://");

            if (dir.DirExists(SAVE_FOLDER_NAME)) return false;

            dir.MakeDir(SAVE_FOLDER_NAME);
            return true;
        }

        public bool SaveDataExists(string forWorldId)
        {
            var file = new File();
            string path = $"{SAVE_FOLDER_PATH}{forWorldId}{EXTENSION}";

            bool exists = file.FileExists(path);
            file.Close();
            return exists;
        }

        private void CacheSaveData(string forWorldId, WorldSaveData data)
        {
            _cachedSaveData.Add(forWorldId, data);
        }

        private bool IsCached(string worldId)
        {
            return _cachedSaveData.ContainsKey(worldId);
        }

        private WorldSaveData LoadCached(string worldId)
        {
            _logger.Info($"Loading save data from cache for {worldId}...");
            return _cachedSaveData[worldId];
        }

        private Logger _logger = new Logger(nameof(WorldSaveDataHandler));
        private const string SAVE_FOLDER_PATH = "user://worldpack_savedata/";
        private const string SAVE_FOLDER_NAME = "worldpack_savedata";
        private const string EXTENSION = ".jasv";

        private Dictionary<string, WorldSaveData> _cachedSaveData;

        private byte[] _saveDataBuffer = new byte[256];
    }
}