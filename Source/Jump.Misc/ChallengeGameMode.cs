using Godot;
using Jump.Utils;

namespace Jump.Misc
{
    public class ChallengeGameMode : GameModeBase
    {
        public override string Name => TranslationServer.Translate("UI_GAMEMODE_CHALLENGE");

        public override string ObstacleRootName => "ChallengeObstacles";

        public override short MaxHearts => 3;

        public override bool EnableCheats => false;

        public override bool ShouldUseTimer { get; set; } = true;

        public override void OnGameOver()
        {
        }

        public override void OnStoppedPlaying()
        {
            Essence = 0;
        }

        public override void OnRetry()
        {
            Essence = 0;
        }

        public override void OnWin()
        {
            var maxEssence = player.LevelRoot.MaxEssence;
            // var maxExperience = game.CurrentLevel.MaxExperience;

            var heartsBonus = CalculateHeartsBonus(player.HealthHandler.Hearts);
            var collectibleBonus = CalculateCollectibleBonus(maxEssence, Essence);

            // var experience = CalculateExperience(heartsBonus, collectibleBonus, game.CurrentLevel.MaxExperience);

            // progressHandler.Experience += experience;
        }

        public override void EssenceCollected()
        {
            Essence += 1;
            OnEssenceChanged?.Invoke();
        }

        public float CalculateHeartsBonus(int current)
        {
            return (float)current / MaxHearts;
        }

        public float CalculateCollectibleBonus(int max, int collected)
        {
            return (float)collected / max;
        }

        public float CalculateCollectibleBonus(int max)
        {
            return (float)Essence / max;
        }

        public float CalculateBonus(float heartsBonus, float collectibleBonus)
        {
            return heartsBonus * collectibleBonus;
        }

        public int CalculateExperience(float heartsBonus, float collectibleBonus, int maxExperience)
        {
            float totalExperience = CalculateBonus(heartsBonus, collectibleBonus) * maxExperience;
            return (int)totalExperience;
        }
    }
}