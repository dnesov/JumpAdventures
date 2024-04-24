using Godot;

namespace Jump.Unlocks
{
    public class ExperienceUnlockCondition : UnlockConditionBase
    {
        public override bool MeetsCondition()
        {
            return progressHandler.Experience >= _experience;
        }

        [Export] private int _experience;
    }
}