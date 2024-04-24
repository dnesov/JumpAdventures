using Godot;
using Jump.Extensions;

namespace Jump.Entities
{
    public class SerializedCollectible : Collectible
    {
        protected override bool IsCollected()
        {
            var game = this.GetSingleton<Game>();
            return game.GetCurrentLevelSaveData().CollectedCollectibles.Contains(Id);
        }

        protected override void Collected(Player player)
        {
            var game = this.GetSingleton<Game>();
            game.GetCurrentLevelSaveData().CollectCollectible(Id);
        }
    }
}