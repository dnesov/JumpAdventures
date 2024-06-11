using Godot;
using World = Jump.Levels.World;
using Jump.Levels;
using Jump.Utils;
using System;
using GodotFmod;

using Jump.Extensions;
using Jump.Utils.SaveData;
using Jump.Misc;
using Jump.Unlocks;
using System.Globalization;

/// <summary>
/// Stores global game state and provides means of interacting with global state and activities.
/// </summary>
public class Game : Node
{
    public delegate void WinHandler();
    public delegate void LevelRestartHandler();

    public WinHandler OnWin;
    public LevelRestartHandler OnLevelRestart;
    public World CurrentWorld { get; private set; }
    public bool Campaign { get; set; } = true;
    public Level CurrentLevel { get => _currentLevel; private set { } }
    public string CurrentLevelString => $"w{CurrentWorld.Order + 1}l{CurrentWorld.GetLevelIdx(CurrentLevel) + 1}";
    public GameState CurrentState => _currentState;
    public GameState StateBeforePause => _stateBeforePause;
    public bool SplashPlayed { get; set; } = false;
    public bool TimerActive { get; set; }
    public InputMethod LastInputMethod => _lastInputMethod;

    public Action OnQuit;
    public Action OnPaused;
    public Action OnResumed;
    public Action OnRetry;
    public Action OnLateRetry;
    public Action OnSave;
    public Action OnHideUi;
    public Action OnShowUi;
    public Action<string> OnWorldLoaded;

    public int Attempts { get; private set; } = 1;
    public GlobalDataHandler DataHandler { get => _dataHandler; set => _dataHandler = value; }
    public ConfigSaveData Data => DataHandler.Data;

    public WorldSaveDataHandler WorldSaveHandler => _worldSaveHandler;
    public WorldpackScanner WorldpackScanner => _worldpackScanner;
    public GameModeBase CurrentGameMode => _currentGameMode;
    public WorldSaveData CurrentWorldSaveData;

    public float Timer => _timer;
    public string TimerFormatted => string.Format(CultureInfo.InvariantCulture, Constants.TimerFormat, Timer);
    public bool TimerRunning => _timerRunning;
    public bool UseTimer => Data.TimerEnabled;
    public AchievementProvider AchievementProvider => _achievementProvider;
    public ActivityHandler ActivityHander => _activityHandler;

    public bool ShowIntro = true;

    public override async void _Ready()
    {
        _sceneSwitcher = this.GetSingleton<SceneSwitcher>();
        _progressHandler = this.GetSingleton<ProgressHandler>();
        WorldpackScanner.CreateWorldpackFolder();
        DataHandler = new GlobalDataHandler();
        _debug = this.GetSingleton<DebugTools>();

        // TODO: temporary workaround for a startup crash when running the release build.
        // do this interactively in a loading screen instead.
        WorldpackScanner.Scan("res://Levels/", false);

        PauseMode = PauseModeEnum.Process;

        SetGameMode(new AdventureGameMode());
        DataHandler.LoadData();

        await ToSignal(this, "ready");

        var steamManager = this.GetSingleton<SteamManager>();
        _achievementProvider = new SteamAchievementProvider(steamManager);

        var unlocksDb = this.GetSingleton<UnlocksDatabase>();
        unlocksDb.OnUnlockableJustUnlocked += (_) => { _achievementProvider.CheckCompletionistAchievement(unlocksDb); };


        RegisterActivities();
        SetActivities("", "In The Menu");

        ApplySettings();

        SubscribeEvents();
    }

    public void StartTimer()
    {
        if (!TimerActive) return;
        _timerRunning = true;
    }

    public void StopTimer()
    {
        if (!TimerActive) return;
        _timerRunning = false;
    }

    public void ResetTimer()
    {
        _timer = 0f;
        TimerActive = false;
    }

    public void ApplySettings()
    {
        var settings = DataHandler.Data.Settings;
        OS.WindowSize = settings.WindowSize;
        OS.WindowFullscreen = settings.Fullscreen;
        OS.VsyncEnabled = settings.VSync;
        Engine.TargetFps = settings.MaxFps;

        TranslationServer.SetLocale(settings.Locale);
        ApplyAudioSettings();
    }

    public override void _Notification(int what)
    {
        if (what == MainLoop.NotificationWmQuitRequest)
            Quit();
    }

    public override void _Process(float delta)
    {
        _activityHandler.ProcessActivities();

        if (_timerRunning)
        {
            _timer += delta;
        }
    }

    public void EnableCursor()
    {
        if (_lastInputMethod == InputMethod.Controller) return;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public void DisableCursor()
    {
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            TogglePause();
        }

        if (Input.IsActionJustPressed("screenshot"))
        {
            _debug.TakeScreenshot();
        }

        if (Input.IsActionJustPressed("debug_showlocales"))
        {
            TranslationServer.Clear();
            GetTree().Notification(NotificationTranslationChanged);
        }

        // if (Input.IsActionJustPressed("open_feedback_form"))
        // {
        //     var levelName = CurrentLevel != null ? CurrentLevelString : _currentState.ToString();
        //     Constants.OpenFeedbackForm(levelName);
        // }

        if (Input.IsActionJustPressed("ui_cancel") && _currentState == GameState.PlayingOverWin) ReturnToMenu(true);

        // _logger.Info(@event);

        var inputMethod = DetermineInputMethod(@event);

        if (inputMethod != _lastInputMethod)
        {
            _lastInputMethod = inputMethod;
        }

        if (CurrentState == GameState.PlayingOverWin)
        {
            EnableCursor();
        }

        if (inputMethod == InputMethod.Controller)
        {
            DisableCursor();
        }

        var inGame = CurrentState == GameState.Playing || CurrentState == GameState.PlayingOver;

        if (inputMethod == InputMethod.Keyboard && !inGame)
        {
            EnableCursor();
        }


#if DEBUG
        if (Input.IsActionJustPressed("ui_toggle"))
        {
            _uiVisible = !_uiVisible;

            if (_uiVisible)
            {
                OnShowUi?.Invoke();
            }
            else
            {
                OnHideUi?.Invoke();
            }
        }
#endif
    }

    public LevelSaveData GetCurrentLevelSaveData()
    {
        var currentLevelId = CurrentWorld.GetLevelIdx(CurrentLevel);
        return CurrentWorldSaveData.GetLevelSaveAt(currentLevelId);
    }

    public void LoadLevel(Level level, bool updateLevelIdx = true)
    {
        if (CurrentWorld == null) throw new Exception("Current world is not selected!");
        if (updateLevelIdx) CurrentWorld.UpdateLevelIdxFrom(level);

        _currentState = GameState.Playing;
        _previousLevel = _currentLevel;
        _currentLevel = level;

        var currentLevelId = CurrentWorld.GetLevelIdx(CurrentLevel);
        var levelSave = CurrentWorldSaveData.TryGetLevelSaveAt(currentLevelId);
        _logger.Info($"Loading level: {CurrentLevel.Name}.");
        _logger.Info($"Current level completed: {levelSave.Completed}");
        Attempts = levelSave.Attempts;

        _sceneSwitcher.Load(level.GlobalPath, true);

        string levelName = _currentLevel.Name;

        if (levelName == "UI_LEVEL")
        {
            var format = Tr(_currentLevel.Name);
            levelName = $"{string.Format(format, CurrentWorld.GetLevelIdx(_currentLevel) + 1)}";
        }

        SetActivities($"\"{levelName}\" in {Tr(CurrentWorld.Name)}", _currentState.ToString());
    }

    public void ReturnToMenu(bool transition = false)
    {
        _currentState = GameState.InMenu;

        SetActivities("", "In The Menu");

        _sceneSwitcher.Load(Constants.NEW_MAIN_MENU_PATH, transition);

        ResetCurrentLevelProgress();

        StopPlaying();
    }

    public void GoToEditor(bool transition = false)
    {
        _currentState = GameState.Editor;

        SetActivities("", "In The Level Editor");
        _sceneSwitcher.Load(Constants.LEVEL_EDITOR_PATH, transition);

        if (CurrentLevel != null)
        {
            ResetCurrentLevelProgress();
            StopPlaying();
        }
    }

    public void Save()
    {
        OnSave?.Invoke();
        DataHandler.SaveData();
    }

    public void Win()
    {
        _logger.Info("Level won.");


        var database = this.GetSingleton<UnlocksDatabase>();


        _currentState = GameState.PlayingOverWin;

        var currentLevelId = CurrentWorld.GetLevelIdx(CurrentLevel);
        var levelSave = CurrentWorldSaveData.TryGetLevelSaveAt(currentLevelId);

        levelSave.Complete();

        StopTimer();

        Save();

        _currentGameMode.OnWin();

        database.TryUnlockWorlds();
        _achievementProvider.CheckWorldAchievements(_progressHandler);
        OnWin?.Invoke();
    }

    public bool JustStartedPlaying() => _previousLevel == null;

    public bool ShouldShowIntro()
    {
        return ShowIntro && CurrentLevel.ShowIntro;
    }

    public void StopPlaying()
    {
        if (CurrentWorld != null)
            SaveWorldProgress(CurrentWorld.UniqueId);

        CurrentWorld = null;
        _currentLevel = null;
        _previousLevel = null;
        CurrentWorldSaveData = new WorldSaveData();
        CurrentGameMode.OnStoppedPlaying();

        ResetTimer();
        StopTimer();
    }

    public void LoadNextLevel()
    {
        ResetCurrentLevelProgress();
        SaveWorldProgress(CurrentWorld.UniqueId, savePreviousLevel: true);
        if (CurrentWorld.CanGetNextLevel())
            LoadLevel(CurrentWorld.GetNextLevel());
        else
            ReturnToMenu(true);
    }

    public void ResetCurrentLevelProgress()
    {
        Attempts = 1;
        CurrentGameMode.OnStoppedPlaying();
    }

    public void GameOver()
    {
        _currentState = GameState.PlayingOver;
        _logger.Info("Game over!");
        _currentGameMode.OnGameOver();
    }

    public void SetGameMode(GameModeBase gamemode)
    {
        gamemode.Register(this, _progressHandler);
        _currentGameMode = gamemode;
    }

    public void AddAttempts()
    {
        Attempts++;
        _progressHandler.TotalAttempts++;
    }

    public bool TryRetry(bool addAttempts = false)
    {
        // if (_currentState == GameState.PlayingOverWin) return false;
        _logger.Info("Retrying.");

        if (addAttempts) AddAttempts();

        if (_currentState == GameState.PlayingOverWin)
        {
            Attempts = 1;
        }

        CurrentGameMode.OnRetry();

        OnRetry?.Invoke();
        _currentState = GameState.Playing;
        return true;
    }

    public void LateRetry()
    {
        _logger.Info("Late retry.");

        _currentGameMode.OnLateRetry();
        ResetTimer();
        OnLateRetry?.Invoke();
    }

    public bool TryRestartLevel()
    {
        if (_currentState != GameState.PlayingOver) return false;
        ForceRestartLevel();
        return true;
    }

    public void Pause()
    {
        if (_currentState == GameState.Paused) return;

        _stateBeforePause = _currentState;
        _currentState = GameState.Paused;

        OnPaused?.Invoke();
        GetTree().Paused = true;
        VisualServer.SetShaderTimeScale(0f);
        StopTimer();
    }

    public void Resume()
    {
        if (_currentState != GameState.Paused) return;
        GetTree().Paused = false;
        _currentState = _stateBeforePause;
        OnResumed?.Invoke();
        VisualServer.SetShaderTimeScale(1f);

        if (_currentState != GameState.PlayingOver)
            StartTimer();
    }

    public void ForceRestartLevel()
    {
        OnLevelRestart?.Invoke();
        _currentState = GameState.Playing;
        CurrentGameMode.OnStoppedPlaying();

        _sceneSwitcher.Reload();
    }

    public void LoadWorld(World world)
    {
        CurrentWorld = world;
        var worldId = world.UniqueId;

        LoadWorldProgress(worldId);
        DataHandler.Data.LastWorldId = worldId;

        OnWorldLoaded?.Invoke(worldId);

        _logger.Info($"World loaded: {CurrentWorld.Name} ({CurrentWorld.UniqueId}).");
    }

    public string GetTranslationInputSuffix()
    {
        if (LastInputMethod == InputMethod.Controller) return "_XBOX";
        return string.Empty;
    }

    private void SubscribeEvents()
    {
        Input.Singleton.Connect("joy_connection_changed", this, nameof(JoyConnectionChanged));
    }

    private void JoyConnectionChanged(int device, bool connected)
    {
        if (_currentState != GameState.Playing) return;
        if (device == 0 && !connected)
        {
            Pause();
        }
    }

    private void TogglePause()
    {
        var paused = _currentState == GameState.Paused;
        bool canPause = _currentState != GameState.PlayingOverWin && _currentState != GameState.InMenu;

        if (paused) Resume();
        else if (canPause) Pause();
    }

    private void SaveWorldProgress(string worldId, bool savePreviousLevel = false)
    {
        var currentLevelId = CurrentWorld.GetLevelIdx(CurrentLevel);
        var levelId = savePreviousLevel ? currentLevelId - 1 : currentLevelId;
        if (levelId < 0) return;
        var levelSave = CurrentWorldSaveData.TryGetLevelSaveAt(levelId);
        levelSave.Attempts = Attempts;
        _worldSaveHandler.TrySaveData(worldId, CurrentWorldSaveData);
    }

    private void ApplyAudioSettings()
    {
        var settings = DataHandler.Data.Settings.AudioSettings;
        var fmodRuntime = this.GetSingleton<FmodRuntime>();
        fmodRuntime.GetBus("bus:/").Volume = settings.MasterVolume;
        fmodRuntime.GetBus("bus:/SFX").Volume = settings.SfxVolume;
        fmodRuntime.GetBus("bus:/Music").Volume = settings.MusicVolume;
    }

    private void LoadWorldProgress(string worldId)
    {
        CurrentWorldSaveData = _worldSaveHandler.TryLoadData(worldId);
    }

    public void SetActivities(string state, string details, string asset = "activity_default", string extras = "")
    {
        var data = new ActivityData
        {
            State = state,
            Details = details,
            Assets = new ActivityAssets
            {
                LargeAsset = asset,
                LargeAssetText = extras
            }
        };
        _activityHandler.CurrentActivity = data;
        _activityHandler.SetActivities(data);
    }

    public void SetActivities(ActivityData data)
    {
        _activityHandler.CurrentActivity = data;
        _activityHandler.SetActivities(data);
    }

    private void RegisterActivities()
    {
        _activityHandler.RegisterActivity(new DiscordActivity());
    }

    private InputMethod DetermineInputMethod(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventKey or InputEventMouse:
                return InputMethod.Keyboard;
            case InputEventJoypadButton:
                return InputMethod.Controller;
            // TODO: make a deadzone const.
            case InputEventJoypadMotion { AxisValue: < -0.5f or > 0.5f }:
                return InputMethod.Controller;
        }

        return _lastInputMethod;
    }

    public void Quit()
    {
        Save();
        _logger.Info($"Quitting the game. Bye bye!");
        OnQuit?.Invoke();

        _activityHandler.Shutdown();

        GetTree().Quit();
    }

    private GameModeBase _currentGameMode;
    private Logger _logger = new Logger(nameof(Game));
    private Level _currentLevel;
    private GameState _currentState;
    private GameState _stateBeforePause;
    private WorldpackScanner _worldpackScanner = new WorldpackScanner();
    private SceneSwitcher _sceneSwitcher;
    private ActivityHandler _activityHandler = new ActivityHandler();
    private SaveDataSerializer _serializer = new SaveDataSerializer();
    private GlobalDataHandler _dataHandler;
    private WorldSaveDataHandler _worldSaveHandler = new();
    private ProgressHandler _progressHandler;

    // TEMP: since the game will only be available on Steam at the moment, just use SteamAchievementProvider.
    private AchievementProvider _achievementProvider;

    private InputMethod _lastInputMethod;

    private Level _previousLevel;
    private DebugTools _debug;

    private float _timer;
    private bool _timerRunning;

    private bool _uiVisible = true;
}