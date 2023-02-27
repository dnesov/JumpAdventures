using Godot;
using YamlDotNet.Serialization;

public class GlobalData
{
    [YamlIgnore] public bool AcceptedNotice = false;
    public bool UseNewMenu = true;
    public SettingsData Settings = new SettingsData();
    public GlobalSaveData SaveData = new GlobalSaveData();
}

public class GlobalSaveData
{
    public int Essence = 0;
    public float Experience = 0;
}

public class SettingsData
{
    public Vector2 WindowSize { get; set; } = new Vector2(1280, 720);
    public bool EnableRichPresence { get; set; } = false;
    public bool Fullscreen { get; set; } = true;
    public bool VSync { get; set; } = true;
    public string Locale { get; set; } = "en";
    public AudioSettings AudioSettings { get; set; } = new AudioSettings();

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