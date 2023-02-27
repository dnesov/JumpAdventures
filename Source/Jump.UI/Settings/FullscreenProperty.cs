using System;
using Godot;

namespace Jump.UI.Settings
{
    public abstract class FullscreenProperty : OptionSettingsProperty
    {
        protected override void OnUpdateElements(GlobalData data)
        {
            optionButton.Select(data.Settings.Fullscreen ? 1 : 0);
        }
        protected override void ValueChanged(int value)
        {
            data.Settings.ToggleFullscreen(value == 1 ? true : false);
            OS.WindowFullscreen = value == 1 ? true : false;
        }
    }
}