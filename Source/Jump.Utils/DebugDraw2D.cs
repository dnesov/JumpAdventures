using Godot;

namespace Jump.Utils
{
    public class DebugDraw2D : Control
    {
        private static DebugDraw2D _instance;
        public static DebugDraw2D Instance
        {
            get
            {
                return _instance;
            }
        }
        public override void _Ready()
        {
            _instance = this;
        }
        public override void _Process(float delta)
        {
            Update();
        }

        public override void _Draw()
        {

        }
    }
}