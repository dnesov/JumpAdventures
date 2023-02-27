using Godot;
using GodotFmod;
using System;

namespace Jump.UI.Settings
{
    public abstract class SfxVolumeProperty : VolumeProperty
    {
        protected override void OnUpdateElements(GlobalData data)
        {
            slider.Value = data.Settings.AudioSettings.SfxVolume;
        }
        protected override void ValueChanged(float value)
        {
            data.Settings.AudioSettings.SfxVolume = value;
            fmodRuntime.GetBus("bus:/SFX").Volume = value;
        }
    }
}
