using Godot;

namespace Jump.Entities
{
    public class CinematicCamera : Camera2D
    {
        public float Acceleration { get => _acceleration; set => _acceleration = value; }
        public float Friction { get => _friction; set => _friction = value; }
        public float MaxSpeed { get => _maxSpeed; set => _maxSpeed = value; }

        public override void _Process(float delta)
        {
            // base._Process(delta);
            // var dir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

            // if (dir.Length() > 0.0f)
            // {
            //     _velocity = _velocity.LinearInterpolate(MaxSpeed * dir, Acceleration * delta);
            // }
            // else
            // {
            //     _velocity = _velocity.LinearInterpolate(Vector2.Zero, Friction * delta);
            // }

            // GlobalPosition += _velocity * delta;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (@event is InputEventMouseMotion mouse)
            {
                if (mouse.ButtonMask == 4)
                {
                    Position -= mouse.Relative * Zoom;
                }
            }

            // if (@event is InputEventMouseButton mouseButton)
            // {
            //     if (mouseButton.ButtonIndex == (int)ButtonList.WheelUp)
            //     {
            //         Zoom -= Vector2.One * 0.05f;
            //     }
            //     else if (mouseButton.ButtonIndex == (int)ButtonList.WheelDown)
            //     {
            //         Zoom += Vector2.One * 0.05f;
            //     }
            // }
        }

        private Vector2 _velocity;
        private float _maxSpeed = 200f;
        private float _acceleration = 5f;
        private float _friction = 5f;
    }
}