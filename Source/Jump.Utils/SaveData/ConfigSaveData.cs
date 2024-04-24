using Godot;
using YamlDotNet.Serialization;

namespace Jump.Utils.SaveData
{
    public class ConfigSaveData : SaveDataBase
    {
        [YamlIgnore] public override string FileName => "config.yaml";
        [YamlIgnore] public bool AcceptedNotice = false;
        public string LastWorldId = "ja_world1_seaside";
        public bool TimerEnabled = false;
        public SettingsData Settings = new SettingsData();
    }

    public class SettingsData
    {
        public Vector2 WindowSize { get; set; } = new Vector2(1280, 720);
        public bool EnableRichPresence { get; set; } = true;
        public bool Fullscreen { get; set; } = true;
        public bool VSync { get; set; } = true;
        public int MaxFps { get; set; } = 60;
        public string Locale { get; set; } = "en";
        public AudioSettings AudioSettings { get; set; } = new AudioSettings();
        public AccessibilitySettings AccessibilitySettings { get; set; } = new AccessibilitySettings();

        public void ToggleFullscreen(bool toggled)
        {
            Fullscreen = toggled;
        }
    }

    public class AudioSettings
    {
        public float MasterVolume = 1f;
        public float SfxVolume = 1f;
        public float MusicVolume = 1f;
    }

    public class AccessibilitySettings
    {
        public float ScreenShake = 1.0f;
        public float ControllerVibration = 1.0f;
        public bool FlashingEnabled = true;
    }
}