using Godot;

namespace Jump.Entities
{
    public class MovementSettings : Resource
    {
        [Export] public readonly float Speed = 450f;
        [Export] public readonly float JumpSpeed = 600f;
        [Export] public readonly float Gravity = 1500f;
        [Export] public readonly float Acceleration = 10f;
        [Export] public readonly float GroundFriction = 10f;
        [Export] public readonly float CoyoteTime = 0.2f;
        [Export] public readonly float JumpBuffer = 0.2f;
    }
}