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

            context.Player.Friction = _dashFriction;
            context.Player.Velocity = _direction * _speed;
        }

        public override void OnExit(StateContext context)
        {

        }

        public override void OnProcess(StateContext context, out Vector2 velocity, float delta)
        {
            velocity = context.StateData.Velocity;
            if (context.StateData.OnGround) context.SwitchTo(new GroundState());

            velocity.y += _movementSettings.Gravity * delta;
        }

        private Vector2 _direction;
        private float _dashFriction = 2.5f;
        private float _speed;
        private MovementSettings _movementSettings;
    }
}