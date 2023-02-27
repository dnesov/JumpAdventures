using Godot;
using Steamworks;
public class SteamManager : Node
{
    public override void _Ready()
    {
        try
        {
            _logger.Info($"Is Steam running? {SteamAPI.IsSteamRunning()}");
            if (SteamAPI.Init())
            {
                _logger.Info("Steamworks initialization succeeded!");
                SteamAPI.RestartAppIfNecessary(new AppId_t { m_AppId = Constants.STEAM_APP_ID });

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

    private Logger _logger = new Logger(nameof(SteamManager));
}