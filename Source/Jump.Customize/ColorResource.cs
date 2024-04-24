using Godot;

namespace Jump.Customize
{
    public class ColorResource : Resource
    {
        [Export] public Color Color = new Color(1f, 1f, 1f, 1f);
    }
}