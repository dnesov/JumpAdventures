using Godot;
using GodotFmod;
using System;

namespace Jump.UI.Settings
{
    public abstract class MasterVolumeProperty : VolumeProperty
    {
        protected override void OnUpdateElements(GlobalData data)
        {
            slider.Value = data.Settings.AudioSettings.MasterVolume;
        }
        protected override void ValueChanged(float value)
        {
            data.Settings.AudioSettings.MasterVolume = value;
            fmodRuntime.GetBus("bus:/").Volume = value;
        }
    }
}
