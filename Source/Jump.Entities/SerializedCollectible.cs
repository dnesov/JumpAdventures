using Godot;

namespace Jump.Entities
{
    public class SerializedCollectible : Collectible
    {
        protected override bool IsCollected()
        {
            var game = GetTree().Root.GetNode<Game>("Game");
            return game.GetCurrentLevelSaveData().CollectedCollectibles.Contains(Id);
        }

        protected override void Collected(Player player)
        {
            var game = GetTree().Root.GetNode<Game>("Game");
            game.GetCurrentLevelSaveData().CollectCollectible(Id);
        }
    }
}