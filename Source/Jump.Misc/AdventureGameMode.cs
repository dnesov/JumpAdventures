using Godot;
using Jump.Utils;

namespace Jump.Misc
{
    public class AdventureGameMode : GameModeBase
    {
        public override string Name => TranslationServer.Translate("UI_GAMEMODE_ADVENTURE");
        public override string ObstacleRootName => string.Empty;
        public override short MaxHearts => 3;
        public override bool EnableCheats => false;
        public override bool ShouldUseTimer { get; set; }

        public override void OnGameOver()
        {
        }

        public override void OnRetry()
        {

        }

        public override void OnLateRetry()
        {
            Essence = 0;
        }

        public override void OnStoppedPlaying()
        {
            Essence = 0;
        }

        public override void EssenceCollected()
        {
            Essence += 1;
            OnEssenceCollected?.Invoke();
        }

        public override void OnWin()
        {
            progressHandler.Essence += Essence;
            game.AchievementProvider.SetStat(StatIds.ESSENCE, progressHandler.Essence);
        }
    }
}