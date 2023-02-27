using Godot;

namespace Jump.Entities
{
    public class AdheasedState : PlayerState
    {
        public override void OnEnter(StateContext context)
        {
            context.Player.Sfx.PlayAdheasedSound();
        }

        public override void OnExit(StateContext context)
        {
        }

        public override void OnProcess(StateContext context, out Vector2 velocity, float delta)
        {
            velocity = Vector2.Zero;
            if (context.StateData.Input.Jumping) context.SwitchTo(new JumpState());
        }
    }
}