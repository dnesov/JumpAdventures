using Godot;

namespace Jump.Entities
{
    [Tool]
    public class DashOrb : OrbBase
    {
        public override void _Ready()
        {
            base._Ready();
            _trajectoryLine = GetNode<Line2D>("Trajectory");
            _debugTrajectoryLine = GetNode<Line2D>("DebugTrajectory");
            _trajectoryLine.SetAsToplevel(true);
            _debugTrajectoryLine.SetAsToplevel(true);
        }
        protected override void OnPlayerEntered(Player player)
        {
            PlaySound(enterEventPath);
            PlayEnterAnimation();
        }

        protected override void OnInteract(Player player)
        {
            var pushDir = Vector2.Right.Rotated(GlobalRotation);
            player.Dash(pushDir, _pushForce, player.Friction / 4);
            PlaySound(interactEventPath);
            PlayInteractAnimation();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            UpdateTrajectory(delta);
            UpdateDebugTrajectory(delta);
        }

        private void UpdateTrajectory(float delta)
        {
            _trajectoryLine.GlobalPosition = GlobalPosition;
            _trajectoryLine.ClearPoints();

            var position = Vector2.Zero;

            var direction = Vector2.Right.Rotated(GlobalRotation);
            var velocity = direction * _pushForce;

            for (int i = 0; i < _maxTrajectoryPoints; i++)
            {
                _trajectoryLine.AddPoint(position);
                velocity.y += _gravity * delta;
                position += velocity * delta;
            }
        }

        private void UpdateDebugTrajectory(float delta)
        {
            if (!Engine.EditorHint) return;
            _debugTrajectoryLine.Visible = _showDebugTrajectory;
            if (!_showDebugTrajectory) return;

            _debugTrajectoryLine.GlobalPosition = GlobalPosition;
            _debugTrajectoryLine.ClearPoints();

            var position = Vector2.Zero;

            var direction = Vector2.Right.Rotated(GlobalRotation);
            var velocity = direction * _pushForce;

            for (int i = 0; i < 300; i++)
            {
                _debugTrajectoryLine.AddPoint(position);
                velocity.y += _gravity * delta;
                position += velocity * delta;
            }
        }

        private Line2D _debugTrajectoryLine;
        private Line2D _trajectoryLine;
        [Export(PropertyHint.Range, "0, 1500, 10")] private float _pushForce = 1500f;
        [Export] private int _maxTrajectoryPoints = 45;
        [Export] private bool _showDebugTrajectory = false;
        private float _gravity = 1500f;
    }
}
