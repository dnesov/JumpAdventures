using Godot;

namespace Jump.Entities
{
    public class StickedState : PlayerState
    {
        public override void OnEnter(StateContext context)
        {
        }

        public override void OnExit(StateContext context)
        {
        }

        public override void OnProcess(StateContext context, out Vector2 velocity, float delta)
        {
            var player = context.Player;
            var normal = GetSurfaceNormal(player);
            velocity = context.StateData.Velocity;
            velocity += context.MovementSettings.Gravity * delta * normal * -1f;
        }

        private Vector2 GetSurfaceNormal(Player player)
        {
            var collision = player.GetLastSlideCollision();
            if (collision == null) return Vector2.Up;
            return collision.Normal;
        }
    }
}