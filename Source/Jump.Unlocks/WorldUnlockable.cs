using Godot;

namespace Jump.Unlocks
{
    public class WorldUnlockable : UnlockableBase
    {
        public override string FormattedDescription => description;
        [Export] private string _worldId;
    }
}