using System;
using Godot;

namespace Jump.UI.Settings
{
    public abstract class RichPresenceProperty : OptionSettingsProperty
    {
        protected override void OnUpdateElements(GlobalData data)
        {
            optionButton.Select(data.Settings.EnableRichPresence ? 1 : 0);
        }
        protected override void ValueChanged(int value)
        {
            data.Settings.EnableRichPresence = value == 1 ? true : false;
        }
    }
}