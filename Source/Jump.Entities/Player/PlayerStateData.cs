using Godot;
using Jump.Utils;

namespace Jump.Entities
{
    public struct PlayerStateData
    {
        public InputData Input;
        public Vector2 Velocity;
        public bool GravityFlipped;
        public bool OnGround;
        public bool WasJumpPressed;
        public bool Adheased;
    }
}