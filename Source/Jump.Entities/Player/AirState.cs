using Godot;
using Jump.Utils;

namespace Jump.Entities
{
    public class AirState : PlayerState
    {
        public override void OnEnter(StateContext context)
        {
            if (context.StateData.Adheased)
            {
                _adheaseTimer = _adheaseTime;
            }

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

            var stateData = context.StateData;
            var input = stateData.Input;
            var gravityFlipped = stateData.GravityFlipped;

            var gravity = gravityFlipped ? -_movementSettings.Gravity : _movementSettings.Gravity;

            velocity = stateData.Velocity;

            if (stateData.Adheased && _adheaseTimer < 0)
            {
                context.SwitchTo(new AdheasedState());
            }

            if (CanJump(input.Jumping)) context.SwitchTo(new JumpState());
            if (stateData.OnGround) context.SwitchTo(new GroundState());

            if (IsMoving(input))
                velocity.x = ApplyAcceleration(velocity.x, input.Direction.x, _movementSettings.Speed, _movementSettings.Acceleration, delta);
            else
                velocity.x = ApplyFriction(velocity.x, context.Player.Friction, delta);

            velocity.y += gravity * delta;


            _adheaseTimer -= delta;
        }

        private float _coyoteTime;
        private float _adheaseTimer;
        private readonly float _adheaseTime = 0.25f;
        private bool CanJump(bool jumping) => jumping && CanCoyoteJump;
        private bool CanCoyoteJump => _coyoteTime > 0;
        private MovementSettings _movementSettings;
    }
}