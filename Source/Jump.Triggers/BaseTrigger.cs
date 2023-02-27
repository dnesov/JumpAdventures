using Godot;
using Jump.Entities;

namespace Jump.Triggers
{
    public abstract class BaseTrigger : Area2D, IObstacle
    {
        public void PlayerEntered(Player player)
        {
            OnEntered(player);
        }

        public void PlayerExited(Player player)
        {
            OnExited(player);
        }

        protected abstract void OnEntered(Player player);
        protected abstract void OnExited(Player player);
    }
}