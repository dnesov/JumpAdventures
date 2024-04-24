using Godot;

namespace Jump.Utils
{
    /// <summary>
    /// Input implemented using Godot's Input API.
    /// </summary>
    public class StandardInputHandler : InputHandler
    {
        public override InputData GetInput()
        {
            var dir = new Vector2();

            if (Input.IsActionPressed("move_left"))
            {
                dir.x -= 1.0f;
            }

            if (Input.IsActionPressed("move_right"))
            {
                dir.x += 1.0f;
            }

            var data = new InputData()
            {
                // Direction = Input.GetVector("move_left", "move_right", "move_down", "move_up"),
                Direction = dir.Normalized(),
                Jumping = Input.IsActionJustPressed("move_jump")
            };
            return data;
        }
    }
}