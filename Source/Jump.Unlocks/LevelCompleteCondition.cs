using Godot;
using Jump.Utils;

namespace Jump.Unlocks
{
    public class LevelCompleteCondition : UnlockConditionBase
    {
        public override bool MeetsCondition()
        {
            return progressHandler.TotalLevelsComplete >= _levelCount;
        }

        [Export] private int _levelCount = 20;
    }
}