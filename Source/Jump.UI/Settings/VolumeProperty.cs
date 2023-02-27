using GodotFmod;

namespace Jump.UI.Settings
{
    public abstract class VolumeProperty : SliderProperty
    {
        public override void _Ready()
        {
            base._Ready();
            fmodRuntime = GetTree().Root.GetNode<FmodRuntime>("FmodRuntime");
        }

        protected FmodRuntime fmodRuntime;
    }
}