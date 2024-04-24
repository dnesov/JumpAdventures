using Steamworks;

public class SteamAchievementProvider : AchievementProvider
{
    public SteamAchievementProvider(SteamManager steamManager)
    {
        _manager = steamManager;
    }

    public override bool IsAchievementUnlocked(string achievementId)
    {
        if (!_manager.IsInitialized || !_manager.IsSteamRunning) return false;
        bool achieved;
        SteamUserStats.GetAchievement(achievementId, out achieved);
        return achieved;
    }

    public override bool TryResetAchievements()
    {
        if (!_manager.IsInitialized || !_manager.IsSteamRunning) return false;
        return SteamUserStats.ResetAllStats(true);
    }

    public override bool TrySetAchievement(string achievementId)
    {
        if (!_manager.IsInitialized || !_manager.IsSteamRunning) return false;
        var success = SteamUserStats.SetAchievement(achievementId);
        SteamUserStats.StoreStats();
        return success;
    }

    public override bool SetStat(string statId, int value)
    {
        if (!_manager.IsInitialized || !_manager.IsSteamRunning) return false;
        var success = SteamUserStats.SetStat(statId, value);
        SteamUserStats.StoreStats();
        return success;
    }

    public override bool AddStat(string statId, int amount)
    {
        if (!_manager.IsInitialized || !_manager.IsSteamRunning) return false;

        int currentValue;

        if (SteamUserStats.GetStat(statId, out currentValue))
        {
            int newValue = currentValue + amount;
            SetStat(statId, newValue);
        }
        else
        {
            _logger.Error($"Failed to add stat for {statId}!");
            return false;
        }

        SteamUserStats.StoreStats();

        return true;
    }

    private SteamManager _manager;
    private Logger _logger = new Logger(nameof(SteamAchievementProvider));
}
