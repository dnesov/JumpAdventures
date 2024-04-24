using Godot;
using Jump.Utils;

namespace Jump.Unlocks
{
    public class FragmentUnlockCondition : UnlockConditionBase
    {
        public int FragmentsRequired => _fragmentsRequired;
        public override bool MeetsCondition()
        {
            return progressHandler.GlobalFragments >= _fragmentsRequired;
        }

        [Export] private int _fragmentsRequired;
    }
}