using Godot;
using Jump.Utils;

namespace Jump.Unlocks
{
    public class LevelCompleteCondition : UnlockConditionBase
    {
        public override bool MeetsCondition()
        {
            return false;
        }

        [Export] private string _worldId;
        [Export] private string _levelIdx;
    }
}