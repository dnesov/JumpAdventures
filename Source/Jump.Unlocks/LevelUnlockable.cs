using Godot;

namespace Jump.Unlocks
{
    public class LevelUnlockable : UnlockableBase
    {
        public override string FormattedDescription => description;
        [Export] private string _worldId;
        [Export] private string _levelIdx;
    }
}