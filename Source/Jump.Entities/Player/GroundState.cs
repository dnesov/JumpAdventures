using Godot;


namespace Jump.Entities
{
    public class GroundState : PlayerState
    {
        public override void OnEnter(StateContext context)
        {
            var player = context.Player;

            _movementSettings = context.MovementSettings;
            var yVel = Mathf.Abs(player.VelocityBeforeLanding.y);

            player.SnapVector = player.GravityFlipped ? player.GroundSnapVector * -1f : player.GroundSnapVector;

            if (context.StateData.WasJumpPressed) context.SwitchTo(new JumpState());
            if (yVel < player.LandThreshold) return;
            player.OnLanded?.Invoke();

            var col = player.GetLastSlideCollision();
            if (col != null && col.Collider is ObstacleStaticBody body)
            {
                body.PlayerLanded(player);
            }
        }

        public override void OnExit(StateContext context)
        {

        }

        public override void OnProcess(StateContext context, out Vector2 velocity, float delta)
        {
            var input = context.StateData.Input;
            velocity = context.StateData.Velocity;

            if (context.StateData.WasJumpPressed) context.SwitchTo(new JumpState());
            if (!context.StateData.OnGround) context.SwitchTo(new AirState());

            var dir = context.StateData.Input.Direction.x;


            if (IsMoving(input))
                velocity.x = ApplyAcceleration(velocity.x, input.Direction.x, _movementSettings.Speed, _movementSettings.Acceleration, delta);
            else
                velocity.x = ApplyFriction(velocity.x, _movementSettings.GroundFriction, delta);
        }
        private MovementSettings _movementSettings;
    }
}