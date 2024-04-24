using Godot;
using Jump.Utils;

namespace Jump.Unlocks
{
    public class WorldCompleteCondition : UnlockConditionBase
    {
        public override bool MeetsCondition()
        {
            return progressHandler.GetWorldProgress(_worldId).LevelsCompleted == 20;
        }

        [Export] private string _worldId = "";
    }
}