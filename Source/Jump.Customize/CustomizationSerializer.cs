using Godot;
using Ceras;

namespace Jump.Customize
{
    public class CustomizationSerializer
    {
        public CustomizationPreferences Deserialize()
        {
            var file = new File();
            Error error = file.Open(PREFS_SAVE_PATH, File.ModeFlags.Read);

            if (error != Error.Ok)
                throw new System.IO.FileNotFoundException($"Cannot read unlocks save data at {PREFS_SAVE_PATH}!");

            var length = (long)file.GetLen();
            _saveBuffer = file.GetBuffer(length);
            file.Close();
            var deserializer = new CerasSerializer();
            var data = deserializer.Deserialize<CustomizationPreferences>(_saveBuffer);
            return data;
        }

        public bool Serialize(CustomizationPreferences data)
        {
            var file = new File();
            Error error = file.Open(PREFS_SAVE_PATH, File.ModeFlags.Write);

            if (error != Error.Ok)
                throw new System.IO.FileNotFoundException($"Cannot write unlocks save data at {PREFS_SAVE_PATH}!");

            var serializer = new CerasSerializer();
            serializer.Serialize<CustomizationPreferences>(data, ref _saveBuffer);

            file.StoreBuffer(_saveBuffer);
            file.Close();

            return true;
        }

        public bool SaveFileExists()
        {
            var file = new File();
            return file.FileExists(PREFS_SAVE_PATH);
        }

        private const string PREFS_SAVE_PATH = "user://player_prefs.jasv";
        private byte[] _saveBuffer;
    }
}