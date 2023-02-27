using Godot;

namespace Jump.Entities
{
    public class IdleState : PlayerState
    {

        public override void OnEnter(StateContext context)
        {
            if (context.StateData.OnGround) context.SwitchTo(new GroundState());
            if (!context.StateData.OnGround) context.SwitchTo(new AirState());
        }

        public override void OnExit(StateContext context)
        {
        }

        public override void OnProcess(StateContext context, out Vector2 velocity, float delta)
        {
            velocity = context.StateData.Velocity;
            return;
        }
    }
}