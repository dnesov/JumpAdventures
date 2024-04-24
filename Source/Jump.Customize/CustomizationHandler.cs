using System;
using System.Collections.Generic;
using Godot;
using Jump.Extensions;

namespace Jump.Customize
{
    public class CustomizationHandler : Node
    {
        public Action<CustomizationPreferences> OnPreferencesChanged;

        public CustomizationPreferences Preferences => _preferences;

        public IReadOnlyDictionary<string, Color> Colors => _colors;
        public IReadOnlyDictionary<string, Texture> Skins => _skins;
        public IReadOnlyDictionary<string, PackedScene> Trails => _trails;

        public override void _Ready()
        {
            base._Ready();
            var game = this.GetSingleton<Game>();

            game.OnQuit += OnQuit;

            _serializer = new CustomizationSerializer();
            var prefs = _serializer.SaveFileExists() ? _serializer.Deserialize()
            : new CustomizationPreferences();

            UpdatePreferences(prefs);
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

        public PackedScene GetTrailSceneById(string trailId)
        {
            if (Trails.Count == 0) return null;
            return Trails[trailId];
        }

        public void UpdatePreferences(CustomizationPreferences prefs)
        {
            _preferences = prefs;
            OnPreferencesChanged?.Invoke(_preferences);
        }

        private void OnQuit()
        {
            if (_preferences == null) return;
            _serializer.Serialize(_preferences);
        }

        [Export] private Dictionary<string, Texture> _skins = new Dictionary<string, Texture>();
        [Export] private Dictionary<string, Color> _colors = new Dictionary<string, Color>();
        [Export] private Dictionary<string, PackedScene> _trails = new();
        private CustomizationPreferences _preferences;
        private CustomizationSerializer _serializer;
    }
}