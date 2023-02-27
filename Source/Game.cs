using Godot;
using World = Jump.Levels.World;
using Jump.Levels;
using Jump.Utils;
using System.Collections.Generic;
using System;
using System.Linq;
using GodotFmod;

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
    public Level CurrentLevel { get => _currentLevel; private set { } }
    public GameState CurrentState { get => _currentState; private set { } }
    public bool SplashPlayed { get; set; } = false;
    private int _sessionEssence = 0;


    public Action OnQuit;
    public Action OnPaused;
    public Action OnResumed;
    public Action OnRetry;
    public Action OnHideUi;
    public Action OnShowUi;

    public int Attempts { get; private set; } = 1;
    public GlobalDataHandler DataHandler { get => _dataHandler; set => _dataHandler = value; }
    public int SessionEssence
    {
        get => _sessionEssence; set
        {
            _sessionEssence = value;
            OnEssenceChanged?.Invoke();
        }
    }

    public WorldSaveDataHandler WorldSaveHandler { get => _worldSaveHandler; set => _worldSaveHandler = value; }
    public Action OnEssenceChanged;
    public WorldSaveData CurrentWorldSaveData;

    public override void _Ready()
    {
        RegisterActivities();
        SetActivities("", "In The Menu");

        _debug = GetTree().Root.GetNode<DebugTools>("DebugTools");
        _sceneSwitcher = GetTree().Root.GetNode<SceneSwitcher>("SceneSwitcher");
        _worldpackScanner.CreateWorldpackFolder();
        DataHandler = new GlobalDataHandler();

        _worldSaveHandler = new WorldSaveDataHandler();

        PauseMode = PauseModeEnum.Process;

        LoadSave();
        ApplySettings();
    }

    public void ApplySettings()
    {
        var settings = DataHandler.Data.Settings;
        OS.WindowSize = settings.WindowSize;
        OS.WindowFullscreen = settings.Fullscreen;
        OS.VsyncEnabled = settings.VSync;
        TranslationServer.SetLocale(settings.Locale);
        ApplyAudioSettings();
    }

    public override void _Notification(int what)
    {
        if (what == MainLoop.NotificationWmQuitRequest)
            Quit();
    }

    private void ApplyAudioSettings()
    {
        var settings = DataHandler.Data.Settings.AudioSettings;
        var fmodRuntime = GetTree().Root.GetNode<FmodRuntime>("FmodRuntime");
        fmodRuntime.GetBus("bus:/").Volume = settings.MasterVolume;
        fmodRuntime.GetBus("bus:/SFX").Volume = settings.SfxVolume;
        fmodRuntime.GetBus("bus:/Music").Volume = settings.MusicVolume;
    }

    public override void _Process(float delta)
    {
        _activityHandler.ProcessActivities();
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

        if (Input.IsActionJustPressed("ui_cancel") && _currentState == GameState.PlayingOverWin) ReturnToMenu(true);

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

    private void TogglePause()
    {
        var paused = _currentState == GameState.Paused;
        bool canPause = _currentState != GameState.PlayingOverWin && _currentState != GameState.InMenu;

        if (paused) Resume();
        else if (canPause) Pause();
    }

    private void Save() => DataHandler.SaveData();
    private void LoadSave() => DataHandler.LoadData();

    public void Win()
    {
        _logger.Info("Level won.");
        _currentState = GameState.PlayingOverWin;

        var levelId = GetCurrentLevelIdx();
        var levelSave = CurrentWorldSaveData.TryGetLevelSaveAt(levelId);
        levelSave.Complete();
        OnWin?.Invoke();
        Save();
    }

    public bool JustStartedPlaying() => _previousLevel == null;

    public bool ShouldShowIntro()
    {
        return CurrentLevel.ShowIntro;
    }

    public void StopPlaying()
    {
        if (CurrentWorld != null)
            SaveWorldProgress(CurrentWorld.UniqueId);

        CurrentWorld = null;
        _currentLevel = null;
        _previousLevel = null;
        CurrentWorldSaveData = new WorldSaveData();
        SessionEssence = 0;
    }

    public void LoadNextLevel()
    {
        ResetCurrentLevelProgress();
        SaveWorldProgress(CurrentWorld.UniqueId, true);
        if (CurrentWorld.CanGetNextLevel())
            LoadLevel(CurrentWorld.GetNextLevel());
        else
            ReturnToMenu(true);
    }

    public void ResetCurrentLevelProgress()
    {
        Attempts = 1;
        _dataHandler.Data.SaveData.Essence += SessionEssence;
        SessionEssence = 0;
    }

    public void GameOver()
    {
        _currentState = GameState.PlayingOver;
        _logger.Info("Game over!");
    }

    public bool TryRetry(bool addAttempts = false)
    {
        if (_currentState == GameState.PlayingOverWin) return false;
        _logger.Info("Retrying.");
        _currentState = GameState.Playing;

        if (addAttempts)
            Attempts++;

        SessionEssence = 0;
        OnRetry?.Invoke();
        return true;
    }

    public bool TryRestartLevel()
    {
        if (_currentState != GameState.PlayingOver) return false;
        ForceRestartLevel();
        return true;
    }

    public void Pause()
    {
        _currentState = GameState.Paused;
        OnPaused?.Invoke();
        GetTree().Paused = true;
        VisualServer.SetShaderTimeScale(0f);
    }

    public void Resume()
    {
        if (_currentState != GameState.Paused) return;
        GetTree().Paused = false;
        _currentState = GameState.Playing;
        OnResumed?.Invoke();
        VisualServer.SetShaderTimeScale(1f);
    }

    public void HideUi()
    {
        // OnHideUi?.Invoke();
    }

    public void ShowUi()
    {
        // OnShowUi?.Invoke();
    }

    public void ForceRestartLevel()
    {
        OnLevelRestart?.Invoke();
        _currentState = GameState.Playing;
        SessionEssence = 0;

        _sceneSwitcher.Reload();
    }

    private bool TrySaveWorldData(string forWorldId, WorldSaveData saveData)
    {
        var handler = new WorldSaveDataHandler();
        _logger.Info($"Saving world data for {forWorldId}.");
        return handler.TrySaveData(forWorldId, saveData);
    }

    private bool WorldSaveDataExists(string forWorldId)
    {
        var handler = new WorldSaveDataHandler();
        return handler.SaveDataExists(forWorldId);
    }

    public void LoadWorld(World world)
    {
        CurrentWorld = world;
        LoadWorldProgress(world.UniqueId);
        _logger.Info($"World loaded: {CurrentWorld.Name} ({CurrentWorld.UniqueId}).");
    }

    private void SaveWorldProgress(string worldId, bool savePreviousLevel = false)
    {
        var levelId = savePreviousLevel ? GetCurrentLevelIdx() - 1 : GetCurrentLevelIdx();
        if (levelId < 0) return;
        var levelSave = CurrentWorldSaveData.TryGetLevelSaveAt(levelId);
        levelSave.Attempts = Attempts;
        _worldSaveHandler.TrySaveData(worldId, CurrentWorldSaveData);
    }

    private void LoadWorldProgress(string worldId)
    {
        CurrentWorldSaveData = _worldSaveHandler.TryLoadData(worldId);
    }

    private int GetCurrentLevelIdx()
    {
        return CurrentWorld.GetLevelIdx(CurrentLevel);
    }

    public World[] GetWorldpacks(string directoryPath)
    {
        var result = _worldpackScanner.Scan(directoryPath);
        return result;
    }

    public List<World> GetWorldpacksOrdered(string directoryPath)
    {
        var worlds = GetWorldpacks(directoryPath).OrderBy(w => w.Order).ToList();
        return worlds;
    }

    public LevelSaveData GetCurrentLevelSaveData()
    {
        return CurrentWorldSaveData.GetLevelSaveAt(GetCurrentLevelIdx());
    }

    public void LoadLevel(Level level, bool updateLevelIdx = true)
    {
        if (CurrentWorld == null) throw new Exception("Current world is not selected!");
        if (updateLevelIdx) CurrentWorld.UpdateLevelIdxFrom(level);

        _currentState = GameState.Playing;
        _previousLevel = _currentLevel;
        _currentLevel = level;

        var levelId = GetCurrentLevelIdx();
        var levelSave = CurrentWorldSaveData.TryGetLevelSaveAt(levelId);
        _logger.Info($"Loading level: {CurrentLevel.Name}.");
        _logger.Info($"Current level completed: {levelSave.Completed}");
        Attempts = levelSave.Attempts;

        _sceneSwitcher.Load(level.GlobalPath, true, TransitionType.Fade);

        SetActivities($"\"{_currentLevel.Name}\" in {CurrentWorld.Name}", _currentState.ToString(), "activity_playing");
    }

    public void ReturnToMenu(bool transition = false)
    {
        _currentState = GameState.InMenu;

        SetActivities("", "In The Menu");
        _sceneSwitcher.Load(DataHandler.Data.UseNewMenu ? Constants.NEW_MAIN_MENU_PATH : Constants.OLD_MAIN_MENU_PATH, transition);
        ResetCurrentLevelProgress();
        StopPlaying();
    }

    private void SetActivities(string state, string details, string asset = "activity_idle", string extras = "")
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

        _activityHandler.SetActivities(data);
    }

    private void RegisterActivities()
    {
        // _activityHandler.RegisterActivity(new DiscordActivity());
    }

    public void Quit()
    {
        Save();
        _logger.Info($"Quitting the game. Bye bye!");
        OnQuit?.Invoke();
        GetTree().Quit();
    }

    private Logger _logger = new Logger(nameof(Game));
    private Level _currentLevel;
    private GameState _currentState;
    private WorldpackScanner _worldpackScanner = new WorldpackScanner();
    private SceneSwitcher _sceneSwitcher;
    private ActivityHandler _activityHandler = new ActivityHandler();
    private SaveDataSerializer _serializer = new SaveDataSerializer();
    private GlobalDataHandler _dataHandler;
    private WorldSaveDataHandler _worldSaveHandler;

    private Level _previousLevel;

    private DebugTools _debug;
    private bool _uiVisible = true;
}