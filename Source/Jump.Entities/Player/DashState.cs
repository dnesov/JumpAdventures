using Godot;

namespace Jump.Entities
{
    public class DashState : PlayerState
    {
        public DashState(Vector2 direction, float speed)
        {
            _direction = direction;
            _speed = speed;
        }
        public override void OnEnter(StateContext context)
        {
            _movementSettings = context.MovementSettings;

            context.Player.SnapVector = Vector2.Zero;
            context.Player.Friction = _dashFriction;
            context.Player.Velocity = _direction * _speed;

            context.Player.Sprite.EnableDashEffect();

            _timerRunning = true;
        }

        public override void OnExit(StateContext context)
        {
            _timerRunning = false;
            context.Player.Sprite.DisableDashEffect();
        }

        public override void OnProcess(StateContext context, out Vector2 velocity, float delta)
        {
            if (_timerRunning)
            {
                _timer += delta;
            }

            velocity = context.StateData.Velocity;

            if (context.StateData.OnGround && _timer >= _timeUntilGrounded)
            {
                _timerRunning = false;
                _timer = 0;
                context.SwitchTo(new GroundState());
            }

            var gravity = context.Player.GravityFlipped ? _movementSettings.Gravity * -1f : _movementSettings.Gravity;
            velocity.y += gravity * delta;
        }

        private Vector2 _direction;
        private float _dashFriction = 2.5f;
        private float _speed;
        private MovementSettings _movementSettings;

        private bool _timerRunning;
        private float _timer;
        private float _timeUntilGrounded = 0.2f;
    }
}