using Godot;

namespace Jump.Entities
{
    public class WindObstacle : ObstacleBase
    {
        protected override void OnPlayerEntered(Player player)
        {
            if (_player != null) return;
            _player = player;
        }

        protected override void OnPlayerExited(Player player)
        {

        }

        public override void _PhysicsProcess(float delta)
        {
            if (!playerInside) return;
            var dir = Vector2.Right.Rotated(GlobalRotation) * -1f;
            _player.Velocity += dir * _windForce;
        }

        [Export] private float _windForce = 40f;
        private Player _player;
    }
}