using Godot;

namespace GodotFmod
{
    public class FmodEventListener2D : Node2D
    {
        public override void _Ready()
        {
            _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            var pos = new Vector3()
            {
                x = GlobalPosition.x,
                y = GlobalPosition.y,
                z = 0.0f
            };

            _fmodRuntime.ListenerPosition = pos;
        }

        private FmodRuntime _fmodRuntime;
    }
}