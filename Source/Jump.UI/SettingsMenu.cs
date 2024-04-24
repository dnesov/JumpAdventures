using Godot;
using GodotFmod;
using Jump.Extensions;
using System;

public class SettingsMenu : MarginContainer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _game = this.GetSingleton<Game>();
        _fmodRuntime = this.GetSingleton<FmodRuntime>();
        UpdateElements();
    }

    private void UpdateElements()
    {
        // Video
        UpdateResolutionButton();
        UpdateFullscreenButton();
        UpdateVSyncButton();

        // Audio
        UpdateMasterVolumeSlider();
        UpdateSfxVolumeSlider();
        UpdateMusicVolumeSlider();
    }

    private void UpdateResolutionButton()
    {
        var button = GetNode<OptionButton>("%ResolutionOptionButton");
        button.Items.Clear();

        foreach (var size in _windowSizes)
        {
            button.AddItem($"{size.x}x{size.y}");
        }

        var windowSize = _game.DataHandler.Data.Settings.WindowSize;
        var idx = Array.IndexOf(_windowSizes, windowSize);
        button.Select(idx);
    }

    private void UpdateFullscreenButton()
    {
        var button = GetNode<CheckBox>("%FullscrenCheckbox");
        button.Pressed = _game.DataHandler.Data.Settings.Fullscreen;
    }

    private void UpdateVSyncButton()
    {
        var button = GetNode<CheckBox>("%VSyncCheckbox");
        button.Pressed = _game.DataHandler.Data.Settings.VSync;
    }

    private void UpdateMasterVolumeSlider()
    {
        var slider = GetNode<HSlider>("%MasterVolumeSlider");
        slider.Value = _game.DataHandler.Data.Settings.AudioSettings.MasterVolume;
    }

    private void UpdateSfxVolumeSlider()
    {
        var slider = GetNode<HSlider>("%SfxVolumeSlider");
        slider.Value = _game.DataHandler.Data.Settings.AudioSettings.SfxVolume;
    }

    private void UpdateMusicVolumeSlider()
    {
        var slider = GetNode<HSlider>("%MusicVolumeSlider");
        slider.Value = _game.DataHandler.Data.Settings.AudioSettings.MusicVolume;
    }

    private void ToggleFullscreen(bool toggled)
    {
        OS.WindowFullscreen = toggled;
        _game.DataHandler.Data.Settings.ToggleFullscreen(toggled);
        _logger.Info($"Fullscreen: {toggled}");
    }

    private void ToggleVSync(bool toggled)
    {
        OS.VsyncEnabled = toggled;
        _game.DataHandler.Data.Settings.VSync = toggled;
    }

    // private void ChangeWindowSize(Vector2 size)
    // {
    //     OS.WindowSize = size;
    //     var data = _game.DataHandler.Data;
    //     data.Settings.WindowSize = size;
    //     _logger.Info($"Window size selected: {size.x}x{size.y}");
    // }

    private void ChangeWindowSize(int sizeIdx)
    {
        // ChangeWindowSize(_windowSizes[sizeIdx]);
        var size = _windowSizes[sizeIdx];
        OS.WindowSize = size;
        var data = _game.DataHandler.Data;
        data.Settings.WindowSize = size;
        _logger.Info($"Window size selected: {size.x}x{size.y}");
    }

    private void SetMasterVolume(float volume)
    {
        _fmodRuntime.GetBus("bus:/").Volume = volume;
        _game.DataHandler.Data.Settings.AudioSettings.MasterVolume = volume;
    }

    private void SetSfxVolume(float volume)
    {
        _fmodRuntime.GetBus("bus:/SFX").Volume = volume;
        _game.DataHandler.Data.Settings.AudioSettings.SfxVolume = volume;
    }

    private void SetMusicVolume(float volume)
    {
        _fmodRuntime.GetBus("bus:/Music").Volume = volume;
        _game.DataHandler.Data.Settings.AudioSettings.MusicVolume = volume;
    }

    private void OpenWorldpackFolder()
    {
        OS.ShellOpen($"{OS.GetUserDataDir()}/worldpacks");
    }

    private Logger _logger = new Logger(nameof(SettingsMenu));

    private readonly Vector2[] _windowSizes = new Vector2[]
    {
        new Vector2(320, 180),
        new Vector2(426, 240),
        new Vector2(640, 360),
        new Vector2(848, 480),
        new Vector2(854, 480),
        new Vector2(960, 540),
        new Vector2(1024, 576),
        new Vector2(1280, 720),
        new Vector2(1366, 768),
        new Vector2(1600, 900),
        new Vector2(1920, 1080),
    };

    private readonly int _defaultWindowSize = 7;
    private Game _game;
    private FmodRuntime _fmodRuntime;
}