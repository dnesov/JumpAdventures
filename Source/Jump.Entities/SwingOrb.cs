using Godot;

namespace Jump.Entities
{
    public class SwingOrb : OrbBase
    {
        protected override void OnPlayerEntered(Player player)
        {
            base.OnPlayerEntered(player);
            _player = player;
        }
        protected override void OnPlayerExited(Player player)
        {
            base.OnPlayerExited(player);
            _player = null;
        }

        public override void _Draw()
        {
            base._Draw();
            if (!playerInside) return;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!playerInside || !interacting) return;

            var dir = _player.GlobalPosition.DirectionTo(GlobalPosition);
            var angle = dir.AngleTo(new Vector2(_player.InputDirection.x, 0.0f));

            _player.Velocity += Vector2.Up.Rotated(angle) * _swingStrength;
            Update();
        }

        private Player _player;
        private float _radius = 190f;
        [Export] private float _swingStrength = 100f;
    }
}