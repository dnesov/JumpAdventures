using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;
using GodotFmod;
using ImGuiNET;
using Jump.Customize;
using Jump.Entities;
using Jump.Utils;

public class DebugOverlay : ImGuiNode
{
    private bool _visible = false;
    private bool _showPlayerInspector;
    private bool _showSceneInfo;
    private bool _showWorldpackInfo;
    private bool _showTranslationDebug;
    private bool _showGameInspector;
    private bool _showSoundtrackDebug;
    private bool _showConsoleLog;
    private bool _showFmodStudioDebug;
    private bool _showSavegameInspector;
    private bool _showCustomizeDebug;

    private Game _game;
    private Jump.Levels.World[] _worldpacks;
    private Jump.Levels.World[] _worldpacksUser;

    private FmodRuntime _fmodRuntime;
    private FmodBus _sfxBus;
    private FmodBus _musicBus;

    private float _sfxVolume = 1f;
    private float _musicVolume = 1f;

    private bool _vsync;
    private int _targetFps = 60;

    private CustomizationHandler _customizationHandler;

    private List<Log> _consoleLog = new List<Log>();
    private Dictionary<LogType, Vector4> _logColors = new Dictionary<LogType, Vector4>()
    {
        {LogType.Info, new Vector4(1.0f, 1.0f, 1.0f, 1.0f)},
        {LogType.Warn, new Vector4(1.0f, 1.0f, 0.0f, 1.0f)},
        {LogType.Error, new Vector4(1.0f, 0.0f, 0.0f, 1.0f)},
    };

    private string[] _colorIds = new string[]
    {
        "color_lightblue",
        "color_yellow",
        "color_pink",
        "color_green",
        "color_red",
        "color_white"
    };

    private string[] _skinIds = new string[]
    {
        "skin_chalk",
        "skin_heart",
        "skin_checker"
    };

    public override void Init(ImGuiIOPtr io)
    {
#if DEBUG
        io.Fonts.AddFontFromFileTTF("Hack-Regular.ttf", 16);
        io.Fonts.AddFontDefault();
        ImGuiGD.RebuildFontAtlas();

        _game = GetTree().Root.GetNode<Game>("Game");
        _worldpacks = _game.GetWorldpacks("res://Levels/");
        _worldpacksUser = _game.GetWorldpacks("user://worldpacks/");

        _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
        _customizationHandler = GetTree().Root.GetNode<CustomizationHandler>("CustomizationHandler");
        Logger.OnLog += ConsoleLog;
#endif
    }

    public override void Layout()
    {
#if DEBUG
        _sfxBus = _fmodRuntime.GetBus("bus:/SFX");
        _musicBus = _fmodRuntime.GetBus("bus:/Music");

        if (_visible)
        {
            DrawMainMenuBar();

            if (_showConsoleLog) DrawConsoleLog();
            if (_showPlayerInspector) DrawPlayerInspector();
            if (_showSceneInfo) DrawSceneInfo();
            if (_showWorldpackInfo) DrawWorldpackInfo();
            if (_showTranslationDebug) DrawTranslationDebug();
            if (_showGameInspector) DrawGameInspector();
            if (_showSoundtrackDebug) DrawSoundtrackDebug();
            if (_showFmodStudioDebug) DrawFmodStudioDebug();
            if (_showSavegameInspector) DrawSavegameInspector();
            if (_showCustomizeDebug) DrawCustomizeDebug();
        }

        if (Input.IsActionJustPressed("debug_toggle"))
            ToggleDebug();

        // ImGui.ShowDemoWindow();

        base.Layout(); // this emits the signal
#endif
    }

    public void ToggleDebug()
    {
        _visible = !_visible;
    }

    private void ConsoleLog(string message, LogType logType)
    {
        _consoleLog.Add(new Log() { Message = message, LogType = logType });
    }

    private void DrawMainMenuBar()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Tools"))
            {
                ImGui.Checkbox("Console log", ref _showConsoleLog);
                ImGui.Checkbox("Player Inspector", ref _showPlayerInspector);
                ImGui.Checkbox("Game Inspector", ref _showGameInspector);
                ImGui.Checkbox("Savegame Inspector", ref _showSavegameInspector);
                ImGui.Checkbox("Scene Info", ref _showSceneInfo);
                ImGui.Checkbox("Worldpack Debug", ref _showWorldpackInfo);
                ImGui.Checkbox("Translation Debug", ref _showTranslationDebug);
                ImGui.Checkbox("Soundtrack Debug", ref _showSoundtrackDebug);
                ImGui.Checkbox("FMOD Studio Debug", ref _showFmodStudioDebug);
                ImGui.Checkbox("Customization Debug", ref _showCustomizeDebug);

                ImGui.Separator();
                if (ImGui.Button("Main Menu"))
                {
                    _game.ReturnToMenu();
                }
                if (ImGui.Button("Restart Level"))
                {
                    _game.ForceRestartLevel();
                }
                ImGui.Separator();
                ImGui.LabelText($"{ImGui.GetIO().DisplaySize}", "Display size");
            }

            if (ImGui.BeginMenu("Settings"))
            {
                ImGui.Checkbox("VSync", ref _vsync);
                OS.VsyncEnabled = _vsync;

                if (!_vsync)
                {
                    ImGui.DragInt("Target FPS", ref _targetFps, 0.0f, 10, 420);
                    Engine.TargetFps = _targetFps;
                }
            }

            if (ImGui.Button("Debug notification")) AddDebugNotification();
        }
    }

    private async void AddDebugNotification()
    {
        var notificationManager = GetTree().Root.GetNode<NotificationManager>(nameof(NotificationManager));
        await notificationManager.AddNotification($"{new Random().Next()}", "Nice! New unlock! Or whatever! Test!");
    }

    private void DrawPlayerInspector()
    {
        if (ImGui.Begin("Player Inspector"))
        {
        }
    }

    private void DrawSceneInfo()
    {
        if (ImGui.Begin("Scene Info"))
        {
            ImGui.LabelText($"{Performance.GetMonitor(Performance.Monitor.TimeProcess)} ms", "Process");
            ImGui.LabelText($"{Performance.GetMonitor(Performance.Monitor.TimeFps)}", "FPS");
            ImGui.LabelText($"{Performance.GetMonitor(Performance.Monitor.Render2dDrawCallsInFrame)}", "Draw Calls");
        }
    }

    private void DrawWorldpackInfo()
    {
        if (ImGui.Begin("Worldpack Debug"))
        {
            ImGui.Text("Official:");
            foreach (var world in _worldpacks)
            {
                if (ImGui.Button(world.Name))
                {
                    _game.LoadWorld(world);
                }
            }

            ImGui.Text("Custom:");
            foreach (var world in _worldpacksUser)
            {
                if (ImGui.Button(world.Name))
                {
                    _game.LoadWorld(world);
                }
            }

            ImGui.Separator();

            if (_game.CurrentWorld == null) return;

            ImGui.LabelText("", "Current: " + _game.CurrentWorld.Name);

            if (_game.CurrentWorld.Levels != null)
            {
                foreach (var level in _game.CurrentWorld.Levels)
                {
                    if (ImGui.Button(level.Name))
                    {
                        _game.LoadLevel(level, true);
                    }
                }
            }
        }
    }

    private void DrawTranslationDebug()
    {
        if (ImGui.Begin("Translation Debug"))
        {
            var locales = TranslationServer.GetLoadedLocales();

            foreach (string locale in locales)
            {
                if (ImGui.Button(TranslationServer.GetLocaleName(locale)))
                {
                    TranslationServer.SetLocale(locale);
                }
            }
        }
    }

    private void DrawGameInspector()
    {
        if (ImGui.Begin("Game class Inspector"))
        {
            ImGui.LabelText(_game.CurrentState.ToString(), "State:");
            if (_game.CurrentWorld == null) return;
            ImGui.Separator();
            ImGui.LabelText($"{_game.CurrentWorld.Name} ({_game.CurrentWorld.UniqueId})", "Current World:");
            if (_game.CurrentLevel == null) return;
            ImGui.LabelText(_game.CurrentLevel.Name, "Current Level:");
            ImGui.LabelText(_game.Attempts.ToString(), "Attempts:");
        }
    }

    private void DrawSavegameInspector()
    {
        if (ImGui.Begin("Savegame Inspector"))
        {
            if (_game.CurrentWorld == null)
            {
                ImGui.Text("No world selected!");
                return;
            }
            if (_game.CurrentWorldSaveData == null)
            {
                ImGui.Text("Savedata is not loaded!");
                return;
            }

            if (ImGui.TreeNode(_game.CurrentWorld.UniqueId))
            {
                ImGui.Text($"Unlocked: {_game.CurrentWorldSaveData.Unlocked}");
                ImGui.Text($"Fragments collected: {_game.CurrentWorldSaveData.FragmentsCollected}");

                int count = 0;
                if (ImGui.TreeNode("LevelSaves"))
                {
                    foreach (var levelSave in _game.CurrentWorldSaveData.LevelSaves)
                    {
                        count++;
                        if (levelSave == null) continue;
                        if (ImGui.TreeNode(count.ToString()))
                        {
                            ImGui.Text($"Completed: {levelSave?.Completed}");
                            ImGui.Text($"Attempts: {levelSave?.Attempts}");

                            if (ImGui.TreeNode("Collectibles:"))
                            {
                                foreach (var collectible in levelSave.CollectedCollectibles)
                                {
                                    ImGui.BulletText(collectible.ToString());
                                }
                                ImGui.TreePop();
                            }

                            ImGui.TreePop();
                        }
                    }
                }
                ImGui.TreePop();
            }
        }
    }

    private void DrawSoundtrackDebug()
    {
        if (ImGui.Begin("Soundtrack Debug"))
        {
            var manager = GetTree().Root.GetNode<SoundtrackManager>("SoundtrackManager");

            ImGui.Text($"Intensity: {manager.Intensity}");
        }
    }

    private void DrawConsoleLog()
    {
        if (ImGui.Begin("Console log"))
        {
            if (ImGui.Button("Clear"))
            {
                _consoleLog.Clear();
            }
            foreach (Log log in _consoleLog)
            {
                ImGui.TextColored(_logColors[log.LogType], log.Message);
            }
        }
    }

    private void DrawFmodStudioDebug()
    {
        if (ImGui.Begin("FMOD Studio Debug"))
        {
            ImGui.Text($"DSP: {_fmodRuntime.CpuDspUsage}");
            ImGui.Text($"Memory: {_fmodRuntime.MemoryUsage}");

            ImGui.SliderFloat("SFX", ref _sfxVolume, 0f, 1f);
            ImGui.SliderFloat("Music", ref _musicVolume, 0f, 1f);

            _sfxBus.Volume = _sfxVolume;
            _musicBus.Volume = _musicVolume;
        }
    }

    private void DrawCustomizeDebug()
    {
        if (ImGui.Begin("Customization Debug"))
        {
            ImGui.Text("Colors");
            foreach (string colorId in _colorIds)
            {
                if (ImGui.Button(colorId))
                {
                    _customizationHandler.Preferences.ColorId = colorId;
                    _customizationHandler.UpdatePreferences();
                }
            }
            ImGui.Text("Skins");
            foreach (string skinId in _skinIds)
            {
                if (ImGui.Button(skinId))
                {
                    _customizationHandler.Preferences.SkinId = skinId;
                    _customizationHandler.UpdatePreferences();
                }
            }
        }
    }
}

public struct Log
{
    public string Message;
    public LogType LogType;
}