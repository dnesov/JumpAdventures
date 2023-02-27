using Godot;

namespace Jump.Entities
{
    public abstract class ObstacleBase : Node2D, IObstacle
    {
        public void PlayerEntered(Player player)
        {
            playerInside = true;
            OnPlayerEntered(player);
        }

        public void PlayerExited(Player player)
        {
            playerInside = false;
            OnPlayerExited(player);
        }

        protected abstract void OnPlayerEntered(Player player);
        protected abstract void OnPlayerExited(Player player);

        protected bool playerInside;
    }
}