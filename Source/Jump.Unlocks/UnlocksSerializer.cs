using System.Collections.Generic;
using System.IO;
using Ceras;
using Godot;
using Directory = Godot.Directory;
using File = Godot.File;

namespace Jump.Unlocks
{
    public class UnlocksSerializer
    {
        public Dictionary<string, bool> Deserialize()
        {
            var file = new File();
            Error error = file.Open(UNLOCKS_SAVE_PATH, File.ModeFlags.Read);

            _logger.Info($"Loading unlocks save file at: {UNLOCKS_SAVE_PATH}.");

            if (error != Error.Ok)
                throw new FileNotFoundException($"Cannot read unlocks save data at {UNLOCKS_SAVE_PATH}!");

            var length = (long)file.GetLen();
            _saveBuffer = file.GetBuffer(length);
            file.Close();
            var deserializer = new CerasSerializer();
            var data = deserializer.Deserialize<Dictionary<string, bool>>(_saveBuffer);
            return data;
        }

        public bool Serialize(Dictionary<string, bool> unlocks)
        {
            var file = new File();
            Error error = file.Open(UNLOCKS_SAVE_PATH, File.ModeFlags.Write);

            if (error != Error.Ok)
                throw new FileNotFoundException($"Cannot write unlocks save data at {UNLOCKS_SAVE_PATH}!");

            var serializer = new CerasSerializer();
            serializer.Serialize<Dictionary<string, bool>>(unlocks, ref _saveBuffer);

            file.StoreBuffer(_saveBuffer);
            file.Close();

            return true;
        }

        public bool SaveExists()
        {
            var file = new File();
            bool exists = file.FileExists(UNLOCKS_SAVE_PATH);
            file.Close();
            return exists;
        }

        private Logger _logger = new Logger(nameof(UnlocksSerializer));
        private byte[] _saveBuffer = new byte[1024];
        private const string UNLOCKS_SAVE_PATH = "user://global_unlocks.jasv";
    }
}