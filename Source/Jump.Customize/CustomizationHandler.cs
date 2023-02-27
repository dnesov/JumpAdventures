using System;
using System.Collections.Generic;
using Godot;

namespace Jump.Customize
{
    public class CustomizationHandler : Node
    {
        public Action<CustomizationPreferences> OnPreferencesChanged;

        public CustomizationPreferences Preferences => _preferences;

        public Dictionary<string, Color> Colors { get => _colors; set => _colors = value; }
        public Dictionary<string, Texture> Skins { get => _skins; set => _skins = value; }
        public Dictionary<string, string> SkinIdUnlockId { get => _skinIdUnlockId; set => _skinIdUnlockId = value; }
        public Dictionary<string, string> ColorIdUnlockId { get => _colorIdUnlockId; set => _colorIdUnlockId = value; }

        public override void _Ready()
        {
            base._Ready();
            var game = GetTree().Root.GetNode<Game>("Game");

            game.OnQuit += OnQuit;

            _serializer = new CustomizationSerializer();
            _preferences = _serializer.SaveFileExists() ? _serializer.Deserialize()
            : new CustomizationPreferences();
            UpdatePreferences();
        }

        public Texture GetSkinById(string skinId)
        {
            if (Skins.Count == 0) return null;
            return Skins[skinId];
        }

        public Color GetColorById(string colorId)
        {
            if (Colors.Count == 0) return new Color(1f, 1f, 1f, 1f);
            return Colors[colorId];
        }

        public string GetSkinUnlockId(string skinId)
        {
            if (_skinIdUnlockId == null) return string.Empty;
            if (_skinIdUnlockId.Count == 0) return string.Empty;
            if (!_skinIdUnlockId.ContainsKey(skinId)) return string.Empty;
            if (_skinIdUnlockId[skinId] == null) return string.Empty;

            return _skinIdUnlockId[skinId];
        }

        public string GetColorUnlockId(string colorId)
        {
            if (_colorIdUnlockId == null) return string.Empty;
            if (_colorIdUnlockId.Count == 0) return string.Empty;
            if (!_colorIdUnlockId.ContainsKey(colorId)) return string.Empty;
            if (_colorIdUnlockId[colorId] == null) return string.Empty;

            return _colorIdUnlockId[colorId];
        }

        public void UpdatePreferences()
        {
            OnPreferencesChanged?.Invoke(_preferences);
        }

        private void OnQuit()
        {
            if (_preferences == null) return;
            _serializer.Serialize(_preferences);
        }

        [Export] private Dictionary<string, Texture> _skins = new Dictionary<string, Texture>();
        [Export] private Dictionary<string, Color> _colors = new Dictionary<string, Color>();
        [Export] private Dictionary<string, string> _skinIdUnlockId = new Dictionary<string, string>();
        [Export] private Dictionary<string, string> _colorIdUnlockId = new Dictionary<string, string>();
        private CustomizationPreferences _preferences;
        private CustomizationSerializer _serializer;
    }
}