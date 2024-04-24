using Godot;

namespace Jump.Entities
{
    public class RotatingObstacle : ResetableObstacle
    {
        public override void _Ready()
        {
            base._Ready();
            _initialRotation = Rotation;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            Rotation += _speed * delta;
        }

        protected override void OnPlayerEntered(Player player) { }

        protected override void OnPlayerExited(Player player) { }

        protected override void OnRestart()
        {
            Rotation = _initialRotation;
        }

        private float _initialRotation;
        [Export] private float _speed = 2f;
    }
}