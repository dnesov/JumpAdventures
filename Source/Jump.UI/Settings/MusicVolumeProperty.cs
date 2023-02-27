using Godot;
using GodotFmod;
using System;

namespace Jump.UI.Settings
{
    public abstract class MusicVolumeProperty : VolumeProperty
    {
        protected override void OnUpdateElements(GlobalData data)
        {
            slider.Value = data.Settings.AudioSettings.MusicVolume;
        }
        protected override void ValueChanged(float value)
        {
            data.Settings.AudioSettings.MusicVolume = value;
            fmodRuntime.GetBus("bus:/Music").Volume = value;
        }
    }
}
