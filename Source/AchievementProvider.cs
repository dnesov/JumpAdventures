using System.Collections.Generic;
using Jump.Unlocks;
using Jump.Utils;

public abstract class AchievementProvider
{
    public abstract bool IsAchievementUnlocked(string achievementId);
    public abstract bool TrySetAchievement(string achievementId);
    public abstract bool TryResetAchievements();
    public abstract bool SetStat(string statId, int value);
    public abstract bool AddStat(string statId, int amount);

    public void CheckWorldAchievements(ProgressHandler progressHandler)
    {
        foreach (var worldId in Constants.WorldIds.AsArray)
        {
            var worldProgress = progressHandler.GetWorldProgress(worldId);
            var achievement = _worldIdToAchievementId[worldId];

            if (!IsAchievementUnlocked(achievement))
            {
                if (!worldProgress.IsComplete) continue;
                TrySetAchievement(achievement);
            }
        }
    }

    public void CheckCompletionistAchievement(UnlocksDatabase database)
    {
        int unlockedAmount = 0;

        foreach (var unlockable in database.Unlocks)
        {
            // Is the current unlockable unlocked?
            if (unlockable.Value)
            {
                unlockedAmount += 1;
            }
        }

        if (unlockedAmount == database.Unlocks.Count)
        {
            TrySetAchievement(AchievementIds.GAME_COMPLETIONIST);
            _logger.Info("Unlocking completionist...");
        }
    }

    private Dictionary<string, string> _worldIdToAchievementId = new Dictionary<string, string>()
    {
        {Constants.WorldIds.Seaside, AchievementIds.WORLD1_COMPLETE},
        {Constants.WorldIds.Forests, AchievementIds.WORLD2_COMPLETE},
        {Constants.WorldIds.LavenderFields, AchievementIds.WORLD3_COMPLETE},
    };

    private Logger _logger = new(nameof(AchievementProvider));
}