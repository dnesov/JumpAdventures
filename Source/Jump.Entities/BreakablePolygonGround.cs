using Godot;

namespace Jump.Entities
{
    [Tool]
    public class BreakablePolygonGround : PolygonGround
    {
        public override void _Ready()
        {
            base._Ready();
            var obstacleBody = collisionBody as ObstacleKinematicBody;
            obstacleBody.OnPlayerEntered += PlayerEntered;
            obstacleBody.OnPlayerExited += PlayerExited;
            obstacleBody.OnPlayerLanded += PlayerLanded;
        }

        private void PlayerEntered(Player player)
        {
            if (_player != null) return;
            _player = player;
        }
        private void PlayerExited(Player player) => _player = null;
        private void PlayerLanded(Player player) { }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            if (_player == null) return;
            if (_health <= 0) Destroy();
            _health -= _damageRate * (Mathf.Abs(_player.Velocity.x) / _player.MaxSpeed) * delta;
            GD.Print(_health);
        }

        private void Destroy()
        {
            QueueFree();
        }

        private Player _player;
        [Export] private float _health = 1;
        [Export] private float _damageRate = 0.3f;
    }
}