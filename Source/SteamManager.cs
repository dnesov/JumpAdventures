using Godot;
using Steamworks;
public class SteamManager : Node
{
    public bool IsSteamRunning => SteamAPI.IsSteamRunning();
    public bool IsInitialized => _isSteamInitialized;

    public override void _Ready()
    {
        // if (!Constants.IsSteamBuild) return;
        try
        {
            _logger.Info($"Is Steam running? {IsSteamRunning}");
            if (SteamAPI.Init())
            {
                _logger.Info("Steamworks initialization succeeded!");
                SteamAPI.RestartAppIfNecessary(new AppId_t { m_AppId = Constants.STEAM_APP_ID });
                _isSteamInitialized = true;
            }
            else
            {
                _logger.Warn("Steamworks initialization failed!");
            }
        }
        catch (System.Exception e)
        {
            _logger.Error("Steamworks initialization threw an exception:");
            _logger.Error(e);
        }

        PauseMode = PauseModeEnum.Process;

        GetUserLocale();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (!_isSteamInitialized) return;
        SteamAPI.RunCallbacks();
    }

    public override void _ExitTree()
    {
        // Tell Steam we're done.
        _logger.Info("Shutting down...");
        try
        {
            SteamAPI.Shutdown();
            _logger.Info("Steamworks shutdown succeeded!");
        }
        catch (System.Exception e)
        {
            _logger.Error("Steamworks shutdown threw an exception:");
            _logger.Error(e);
        }
    }

    private void GetUserLocale()
    {
        if (!IsSteamRunning || !_isSteamInitialized) return;
    }

    private void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
    {
        _logger.Warn(pchDebugText);
    }

    private Logger _logger = new Logger(nameof(SteamManager));
    private bool _isSteamInitialized;
}
