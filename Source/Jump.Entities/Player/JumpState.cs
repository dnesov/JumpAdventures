using Godot;
using Jump.Utils;

namespace Jump.Entities
{
    public class JumpState : PlayerState
    {

        public override void OnEnter(StateContext context)
        {
            context.SwitchTo(new AirState());
            context.Player.Jump();
        }

        public override void OnExit(StateContext context) { }

        public override void OnProcess(StateContext context, out Vector2 velocity, float delta)
        {
            velocity = context.StateData.Velocity;
            return;
        }
    }
}