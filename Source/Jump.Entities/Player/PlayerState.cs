using Godot;
using Jump.Utils;

namespace Jump.Entities
{
    public abstract class PlayerState
    {
        public abstract void OnEnter(StateContext context);
        public abstract void OnExit(StateContext context);
        public abstract void OnProcess(StateContext context, out Vector2 velocity, float delta);

        protected float ApplyAcceleration(float velocity, float direction, float speed, float acceleration, float delta) => Mathf.Lerp(velocity, direction * speed, acceleration * delta);
        protected float ApplyFriction(float velocity, float friction, float delta) => Mathf.Lerp(velocity, 0, friction * delta);
        protected bool IsMoving(InputData input) => Mathf.Abs(input.Direction.x) > 0.0f;
    }
}