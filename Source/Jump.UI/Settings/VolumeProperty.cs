using GodotFmod;
using Jump.Extensions;

namespace Jump.UI.Settings
{
    public abstract class VolumeProperty : SliderProperty
    {
        public override void _Ready()
        {
            base._Ready();
            fmodRuntime = this.GetSingleton<FmodRuntime>();
        }

        protected FmodRuntime fmodRuntime;
    }
}