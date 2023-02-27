using Godot;
using Jump.Utils;

namespace Jump.Entities
{
    public class AirState : PlayerState
    {
        public override void OnEnter(StateContext context)
        {
            _movementSettings = context.MovementSettings;
            if (!context.PreviousStateEqualTo(typeof(GroundState))) return;
            _coyoteTime = _movementSettings.CoyoteTime;

        }

        public override void OnExit(StateContext context)
        {

        }

        public override void OnProcess(StateContext context, out Vector2 velocity, float delta)
        {
            _coyoteTime -= delta;

            var input = context.StateData.Input;
            velocity = context.StateData.Velocity;

            if (CanJump(input.Jumping)) context.SwitchTo(new JumpState());
            if (context.StateData.OnGround) context.SwitchTo(new GroundState());

            if (IsMoving(input))
                velocity.x = ApplyAcceleration(velocity.x, input.Direction.x, _movementSettings.Speed, _movementSettings.Acceleration, delta);
            else
                velocity.x = ApplyFriction(velocity.x, context.Player.Friction, delta);

            velocity.y += _movementSettings.Gravity * delta;
        }

        private float _coyoteTime;
        private bool CanJump(bool jumping) => jumping && CanCoyoteJump;
        private bool CanCoyoteJump => _coyoteTime > 0;
        private MovementSettings _movementSettings;
    }
}