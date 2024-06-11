using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;
using GodotFmod;
using ImGuiNET;
using Jump.Customize;
using Jump.Entities;
using Jump.Extensions;
using Jump.Utils;
using Jump.Utils.Animation;
using Jump.Debug;
using Vector2 = Godot.Vector2;
using Jump.Misc;
using Jump.Unlocks;

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
    private bool _showProgressDebug;
    private bool _showUnlocksDebug;
    private bool _showCinematicTools;
    private bool _showReplayTool;

    private Game _game;
    private ProgressHandler _progressHandler;
    private UnlocksDatabase _unlocksDb;
    private List<Jump.Levels.World> _worldpacks;
    private List<Jump.Levels.World> _worldpacksUser;

    private FmodRuntime _fmodRuntime;
    private FmodBus _sfxBus;
    private FmodBus _musicBus;

    private float _sfxVolume = 1f;
    private float _musicVolume = 1f;

    private bool _vsync = true;
    private int _targetFps = 60;

    private CustomizationHandler _customizationHandler;

    private Player _player;

    // Cinematic Tools
    private CinematicCamera _cinematicCam;
    private float _time;
    private float _timeStep = 2f;
    private CameraAnimation _animation = new CameraAnimation();
    private bool _playing;
    private bool _seeking;
    private bool _drawGizmos;
    private Drawer _drawer;


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
        "skin_checker",
        "skin_hexagon",
        "skin_flower",
        "skin_cube",
        "skin_circles",
        "skin_tape",
        "skin_triangles",
        "skin_square",
        "skin_diamond"
    };

    public override void Init(ImGuiIOPtr io)
    {
#if DEBUG
        io.Fonts.AddFontFromFileTTF("Hack-Regular.ttf", 16);
        io.Fonts.AddFontDefault();
        ImGuiGD.RebuildFontAtlas();

        _game = this.GetSingleton<Game>();

        _worldpacks = _game.WorldpackScanner.Scan("res://Levels/");
        _worldpacksUser = _game.WorldpackScanner.Scan("user://worldpacks/");

        _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
        _customizationHandler = this.GetSingleton<CustomizationHandler>();
        _progressHandler = this.GetSingleton<ProgressHandler>();
        _unlocksDb = this.GetSingleton<UnlocksDatabase>();

        Logger.OnLog += ConsoleLog;

        _drawer = new Drawer();
        AddChild(_drawer);
        _drawer.Connect("draw", this, "DrawGizmos");

        _game.OnLateRetry += OnLateRetry;
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
            if (_showProgressDebug) DrawProgressDebug();
            if (_showUnlocksDebug) DrawUnlocksDebug();
            if (_showCinematicTools) DrawCinematicTools();
            if (_showReplayTool) DrawReplayTool();
        }

        if (Input.IsActionJustPressed("debug_toggle"))
            ToggleDebug();


        if (Input.IsActionJustPressed("debug_playall"))
        {
            if (!_animation.IsEmpty)
            {
                ForcePlayCameraAnimation();
            }
        }

        base.Layout(); // this emits the signal
#endif
    }

    private void OnLateRetry()
    {
        if (_replayBuffer.Count > 0 && _shouldPlayReplay)
        {
            // _playingReplay = true;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (_playing || _seeking)
        {
            var seeked = _animation.Seek(_time);
            if (_cinematicCam != null)
            {
                _cinematicCam.GlobalPosition = seeked.Position;
                _cinematicCam.Zoom = seeked.Zoom;
            }
        }

        if (_playing)
        {
            _time += delta;
        }


        if (_recordingReplay || _playingReplay)
        {
            _currentReplayFrame++;
        }

        if (_playingReplay)
        {
            if (_replayBuffer.ContainsKey(_currentReplayFrame))
            {
                var key = _replayBuffer[_currentReplayFrame];
                InputEventAction action = new InputEventAction();
                switch (key)
                {
                    case ReplayKey.StartMoveLeft:
                        action.Action = "move_left";
                        action.Pressed = true;
                        break;
                    case ReplayKey.StartMoveRight:
                        action.Action = "move_right";
                        action.Pressed = true;
                        break;
                    case ReplayKey.EndMoveLeft:
                        action.Action = "move_left";
                        action.Pressed = false;
                        break;
                    case ReplayKey.EndMoveRight:
                        action.Action = "move_right";
                        action.Pressed = false;
                        break;
                    case ReplayKey.StartJump:
                        action.Action = "move_jump";
                        action.Pressed = true;
                        break;
                    case ReplayKey.EndJump:
                        action.Action = "move_jump";
                        action.Pressed = false;
                        break;
                }

                Input.ParseInputEvent(action);
            }
        }
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
                ImGui.Checkbox("Progress Debug", ref _showProgressDebug);
                ImGui.Checkbox("Unlocks Debug", ref _showUnlocksDebug);
                ImGui.Checkbox("Cinematic Tools", ref _showCinematicTools);
                ImGui.Checkbox("Replay Tool", ref _showReplayTool);

                ImGui.Separator();
                if (ImGui.Button("Main Menu"))
                {
                    _game.ReturnToMenu();
                }
                if (ImGui.Button("Restart Level"))
                {
                    _game.ForceRestartLevel();
                }

                if (ImGui.Button("Level Editor"))
                {
                    _game.GoToEditor();
                }

                if (ImGui.Button("Unlock test achievement"))
                {
                    _game.AchievementProvider.TrySetAchievement(AchievementIds.DEBUG_TEST);
                }

                if (ImGui.Button("Reset achievements"))
                {
                    _game.AchievementProvider.TryResetAchievements();
                }

                if (ImGui.Button("Print orphan nodes")) GetTree().Root.PrintStrayNodes();

                ImGui.Separator();
                ImGui.LabelText($"{ImGui.GetIO().DisplaySize}", "Display size");
                ImGui.End();
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
                ImGui.End();
            }

            if (ImGui.Button("Debug notification")) AddDebugNotification();
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        Update();
    }

    public void RegisterPlayer(Player player)
    {
        _player = player;
    }

    private async void AddDebugNotification()
    {
        var notificationManager = this.GetSingleton<NotificationManager>();
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
                int i = 0;
                foreach (var level in _game.CurrentWorld.Levels)
                {
                    i++;
                    ImGui.PushID(level.GetHashCode());

                    string levelName = level.Name;

                    if (levelName == "UI_LEVEL")
                    {
                        var format = Tr(level.Name);
                        levelName = $"{string.Format(format, i)}";
                    }

                    if (ImGui.Button(levelName))
                    {
                        _game.SetGameMode(new AdventureGameMode());
                        _game.LoadLevel(level, true);
                    }
                    ImGui.PopID();
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
            ImGui.LabelText(_game.StateBeforePause.ToString(), "State Before Pause:");
            ImGui.LabelText(_game.TimerActive.ToString(), "Timer Active:");
            ImGui.LabelText(_game.Timer.ToString(), "Timer:");
            ImGui.LabelText(_game.TimerRunning.ToString(), "Timer Running:");

            ImGui.LabelText(_game.LastInputMethod.ToString(), "LastInputMethod:");

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

                            if (ImGui.Button("Set Complete"))
                            {
                                levelSave.Complete();
                            }

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
            var manager = this.GetSingleton<SoundtrackManager>();

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
            foreach (var color in _customizationHandler.Colors)
            {
                if (ImGui.Button(color.Key))
                {
                    var prefs = _customizationHandler.Preferences;
                    prefs.ColorId = color.Key;
                    _customizationHandler.UpdatePreferences(prefs);
                }
            }
            ImGui.Text("Skins");
            foreach (var skin in _customizationHandler.Skins)
            {
                if (ImGui.Button(skin.Key))
                {
                    var prefs = _customizationHandler.Preferences;
                    prefs.SkinId = skin.Key;
                    _customizationHandler.UpdatePreferences(prefs);
                }
            }
        }
    }

    private void DrawProgressDebug()
    {
        if (ImGui.Begin("Progress Debug"))
        {
            int essence = _progressHandler.Essence;
            ImGui.DragInt("Essence", ref essence);
            _progressHandler.Essence = essence;

            int experience = _progressHandler.Experience;
            ImGui.DragInt("Experience", ref experience);
            _progressHandler.Experience = experience;

            int fragments = _progressHandler.GlobalFragments;
            ImGui.DragInt("Fragments", ref fragments);
            _progressHandler.GlobalFragments = fragments;

            ImGui.LabelText("Jumps", _progressHandler.Jumps.ToString());
            ImGui.LabelText("Attempts", _progressHandler.TotalAttempts.ToString());

        }
    }

    private void DrawUnlocksDebug()
    {
        if (ImGui.Begin("Unlocks Debug"))
        {
            foreach (var unlock in _unlocksDb.Unlocks)
            {
                ImGui.LabelText(unlock.Key, unlock.Value.ToString());
            }
        }
    }

    private void DrawCinematicTools()
    {
        if (ImGui.Begin("Cinematic Tools"))
        {
            if (ImGui.Button("Enable"))
            {
                _cinematicCam = new CinematicCamera();
                AddChild(_cinematicCam);

                _cinematicCam.Current = true;

                _player.Camera.Current = false;
                // _player.DisableInput();

                ResetCinematicCamera();

                var keyframe = new CameraKeyframe();
                keyframe.Zoom = _cinematicCam.Zoom;
                keyframe.Position = _cinematicCam.GlobalPosition;
                _animation.AddKeyframe(0.0f, keyframe);
            }

            ImGui.SameLine();

            if (ImGui.Button("Disable"))
            {
                _cinematicCam.Current = false;
                _cinematicCam.QueueFree();

                _player.Camera.Current = true;
                _player.EnableInput();
            }

            ImGui.SameLine();

            if (ImGui.Button("Reset"))
            {
                ResetCinematicCamera();
            }

            if (_cinematicCam != null)
            {
                float zoom = _cinematicCam.Zoom.x;
                ImGui.InputFloat("Zoom:", ref zoom);
                _cinematicCam.Zoom = Vector2.One * zoom;
                ImGui.Checkbox("Gizmos", ref _drawGizmos);

                // Timeline
                ImGui.Separator();
                ImGui.Text("Timeline");

                if (ImGui.Button("+"))
                {
                    AddKeyframe();
                }

                ImGui.SameLine();
                if (ImGui.Button("P"))
                {
                    PlayCameraAnimation();
                }
                ImGui.SameLine();
                ImGui.Checkbox("##_seeking", ref _seeking);
                ImGui.SameLine();
                ImGui.PushItemWidth(64f);
                ImGui.InputFloat("##_timeStep", ref _timeStep);

                ImGui.PopItemWidth();
                if (ImGui.Button("<"))
                {
                    _time = 0.0f;
                }
                ImGui.SameLine();
                ImGui.SliderFloat(string.Empty, ref _time, 0.0f, 100f);
                ImGui.SameLine();
                if (ImGui.Button(">"))
                {
                    _time = _animation.Duration;
                }

                // Keyframes
                for (int i = 0; i < _animation.KeyframeCount; i++)
                {
                    var keyframe = _animation.KeyframeAtIdx(i);

                    ImGui.Text(i.ToString());
                    ImGui.SameLine();
                    if (ImGui.Button($"x##{i}"))
                    {
                        _animation.RemoveKeyframe(i);
                        _playing = false;
                        Update();
                        return;
                    }

                    ImGui.SameLine();

                    if (ImGui.Button($"M##{i}"))
                    {
                        _time = keyframe.StartTime;
                        _seeking = true;
                        _playing = false;
                    }


                    ImGui.SameLine();
                    ImGui.PushItemWidth(128f);

                    float startTime = keyframe.StartTime;
                    if (ImGui.DragFloat($"##{i}", ref startTime))
                    {
                        _animation.UpdateTimeline();
                        keyframe.StartTime = startTime;
                    }

                    System.Numerics.Vector2 preA, postB;

                    // preA = new System.Numerics.Vector2(keyframe.PreA.x, keyframe.PreA.y);
                    // postB = new System.Numerics.Vector2(keyframe.PostB.x, keyframe.PostB.y);

                    // ImGui.DragFloat2($"##{i}_preA", ref preA);
                    // ImGui.DragFloat2($"##{i}_postB", ref postB);

                    // keyframe.PreA = new Vector2(preA.X, preA.Y);
                    // keyframe.PostB = new Vector2(postB.X, postB.Y);
                    ImGui.Text("Zoom:");
                    ImGui.SameLine();

                    float zoomParam = keyframe.Zoom.x;
                    ImGui.InputFloat($"##{i}_zoom", ref zoomParam);
                    keyframe.Zoom = Vector2.One * zoomParam;
                    ImGui.Separator();
                }
            }
        }
    }

    private void AddKeyframe()
    {
        var keyframe = new CameraKeyframe();
        keyframe.Position = _cinematicCam.GlobalPosition;
        // keyframe.PreA = Vector2.Up * 32f;
        // keyframe.PostB = Vector2.Right * 32f;
        keyframe.Zoom = _cinematicCam.Zoom;

        _time += _timeStep;
        _animation.AddKeyframe(_time, keyframe);
        Update();
    }

    private void PlayCameraAnimation()
    {
        if (_animation.IsEmpty) return;
        _playing = !_playing;

        if (_playing)
        {
            _time = 0.0f;
        }
    }

    private void ForcePlayCameraAnimation()
    {
        if (_animation.IsEmpty) return;
        _playing = true;
        _time = 0.0f;
    }

    private void ResetCinematicCamera()
    {
        _cinematicCam.GlobalPosition = _player.SpawnPosition;
        _cinematicCam.Zoom = Godot.Vector2.One;
    }

    private void ResetCamToFirstKeyframe()
    {
        if (_cinematicCam == null) return;
        _playing = false;
        _time = 0.0f;
        var keyframe = _animation.FirstKeyframe;
        _cinematicCam.GlobalPosition = keyframe.Position;
        _cinematicCam.Zoom = keyframe.Zoom;
    }

    private void DrawGizmos()
    {
        if (_cinematicCam == null || !_drawGizmos) return;

        var color = new Color(0.6f, 0.6f, 0.6f);
        var hoverColor = new Color(0.8f, 0.8f, 0.8f);
        var selectColor = new Color(1f, 1f, 1f);

        // Draw path line
        Vector2 prevPos, pos;
        prevPos = _animation.Seek(0f).Position;
        for (int y = 0; y < 100; y++)
        {
            var t = ((float)y / (float)100) * _animation.Duration;
            pos = _animation.Seek(t).Position;
            _drawer.DrawLine(prevPos, pos, color, 2f, true);
            prevPos = pos;
        }

        var mousePos = _drawer.GetLocalMousePosition();

        for (int i = 0; i < _animation.KeyframeCount; i++)
        {
            var current = _animation.KeyframeAtIdx(i);
            var nextIndex = Mathf.Clamp(i + 1, 0, _animation.KeyframeCount - 1);
            var next = _animation.KeyframeAtIdx(nextIndex);

            var distance = mousePos.DistanceTo(current.Position);

            // Draw and select
            var stringPos = new Vector2(current.Position.x, current.Position.y - _debugFont.GetHeight());

            string tooltip = i.ToString();

            // Draw position points
            if (distance < 10)
            {
                if (_selectedKeyframe == i)
                {
                    _drawer.DrawCircle(current.Position, 10f, selectColor);
                }
                else
                {
                    _drawer.DrawCircle(current.Position, 10f, hoverColor);
                    // tooltip = $"{i} (Zoom: ({current.Zoom.x}, {current.Zoom.y})";
                    var stringOffset = stringPos - new Vector2(0, _debugFont.GetHeight());

                    string zoomTooltip = $"Zoom: ({current.Zoom.x}, {current.Zoom.y})";
                    _drawer.DrawString(_debugFont, stringOffset, zoomTooltip);

                    stringOffset -= Vector2.Down * _debugFont.GetHeight();
                    string posTooltip = $"Pos:({current.Position.x}, {current.Position.y})";
                    _drawer.DrawString(_debugFont, stringOffset, posTooltip);
                }

                if (Input.IsMouseButtonPressed((int)ButtonList.Left))
                {
                    _selectedKeyframe = i;
                    _draggingKeyframe = true;
                    _playing = false;
                    _seeking = false;
                }
            }
            else
            {
                if (_selectedKeyframe == i)
                {
                    _drawer.DrawCircle(current.Position, 10f, selectColor);
                }
                else
                {
                    _drawer.DrawCircle(current.Position, 10f, color);
                }

                if (!Input.IsMouseButtonPressed((int)ButtonList.Left) && _draggingKeyframe)
                {
                    _selectedKeyframe = -1;
                    _draggingKeyframe = false;
                }
            }

            _drawer.DrawString(_debugFont, stringPos, tooltip, selectColor);

            // Move
            if (Input.IsMouseButtonPressed((int)ButtonList.Left) && _selectedKeyframe != -1)
            {
                if (_selectedKeyframe == i)
                {
                    current.Position = mousePos;
                }
            }
        }
    }

    private void DrawReplayTool()
    {
        if (ImGui.Begin("Replay Tool"))
        {
            ImGui.Checkbox("Show intro:", ref _game.ShowIntro);
            ImGui.Text($"F: {_currentReplayFrame}");
            if (ImGui.Button("Clear"))
            {
                _currentReplayFrame = 0;
                _replayBuffer.Clear();
            }

            if (ImGui.Button("Play"))
            {
                // PlayReplay();
            }

            ImGui.SameLine();
            if (ImGui.Button("Stop"))
            {
                StopReplay();
            }

            ImGui.Checkbox("Record: ", ref _recordingReplay);
            // ImGui.Checkbox("Should play:", ref _shouldPlayReplay);

            if (_recordingReplay)
            {
                if (Input.IsActionJustPressed("move_left"))
                {
                    _replayBuffer.Add(_currentReplayFrame, ReplayKey.StartMoveLeft);
                }

                if (Input.IsActionJustPressed("move_right"))
                {
                    _replayBuffer.Add(_currentReplayFrame, ReplayKey.StartMoveRight);
                }

                if (Input.IsActionJustReleased("move_left"))
                {
                    _replayBuffer.Add(_currentReplayFrame, ReplayKey.EndMoveLeft);
                }

                if (Input.IsActionJustReleased("move_right"))
                {
                    _replayBuffer.Add(_currentReplayFrame, ReplayKey.EndMoveRight);
                }

                if (Input.IsActionJustPressed("move_jump"))
                {
                    _replayBuffer.Add(_currentReplayFrame, ReplayKey.StartJump);
                }

                if (Input.IsActionJustReleased("move_jump"))
                {
                    _replayBuffer.Add(_currentReplayFrame, ReplayKey.EndJump);
                }
            }

            ImGui.Separator();
            ImGui.Text("Recorded actions:");
            foreach (var replayFrame in _replayBuffer)
            {
                ImGui.Text($"{replayFrame.Key}: {replayFrame.Value}");
            }
        }
    }

    private void PlayReplay()
    {
        Input.FlushBufferedEvents();
        _currentReplayFrame = 0;
        _player.Respawn();
        _player.Velocity = Vector2.Zero;
        _playingReplay = true;
    }

    private void StopReplay()
    {
        Input.FlushBufferedEvents();
        _playingReplay = false;
        _currentReplayFrame = 0;
        _player.Velocity = Vector2.Zero;
        _player.Respawn();
    }

    [Export] private Font _debugFont;

    private int _selectedKeyframe = -1;
    private bool _draggingKeyframe = false;
    private bool _recordingReplay = false;
    private bool _playingReplay = false;
    private bool _shouldPlayReplay = false;

    private Dictionary<int, ReplayKey> _replayBuffer = new();

    private enum ReplayKey
    {
        StartMoveLeft,
        StartMoveRight,
        EndMoveLeft,
        EndMoveRight,
        StartJump,
        EndJump
    }

    private int _currentReplayFrame;
}

public struct Log
{
    public string Message;
    public LogType LogType;
}