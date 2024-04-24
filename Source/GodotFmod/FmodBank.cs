using Godot;

namespace GodotFmod
{
    public class FmodBank : Resource
    {
        [Export(PropertyHint.File, "*.bank")]
        public string Path { get; set; }
    }
}