using Godot;
using Jump.Utils;

namespace Jump.Unlocks
{
    public class EssenceUnlockCondition : UnlockConditionBase
    {
        public int EssenceRequired => _essenceRequired;
        public override bool MeetsCondition()
        {
            return progressHandler.GetCollectibleProgress().Essence >= _essenceRequired;
        }

        [Export] private int _essenceRequired;
    }
}