using Godot;
using Jump.Extensions;

namespace Jump.Entities
{
    public abstract class ObstacleBase : Node2D, IObstacle
    {
        public override void _Ready()
        {
            base._Ready();
            Enable();
        }
        public void PlayerEntered(Player player)
        {
            if (!_enabled) return;
            playerInside = true;
            OnPlayerEntered(player);
        }

        public void PlayerExited(Player player)
        {
            if (!_enabled) return;
            playerInside = false;
            OnPlayerExited(player);
        }

        protected abstract void OnPlayerEntered(Player player);
        protected abstract void OnPlayerExited(Player player);

        public void Enable()
        {
            _enabled = true;
            Visible = true;
        }

        public void Disable()
        {
            _enabled = false;
            Visible = false;
        }

        protected bool playerInside;
        private bool _enabled;
    }
}