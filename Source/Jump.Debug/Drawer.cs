using Godot;

namespace Jump.Debug
{
    public class Drawer : Node2D
    {
        public override void _Process(float delta)
        {
            base._Process(delta);
            Transform = GetViewport().CanvasTransform;
            Update();
        }
    }
}