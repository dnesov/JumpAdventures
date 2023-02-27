using Godot;

namespace GodotFmod
{
    public class FmodEventListener2D : Node2D
    {
        public override void _Ready()
        {
            _fmodRuntime = GetNode<FmodRuntime>("/root/FmodRuntime");
            _fmodRuntime.CurrentListener = this;
        }

        private FmodRuntime _fmodRuntime;
    }
}