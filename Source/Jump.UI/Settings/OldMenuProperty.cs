using System;
using Godot;

namespace Jump.UI.Settings
{
    public abstract class OldMenuProperty : OptionSettingsProperty
    {
        protected override void OnUpdateElements(GlobalData data)
        {
            optionButton.Select(data.UseNewMenu ? 0 : 1);
        }

        protected override void ValueChanged(int value)
        {
            data.UseNewMenu = value == 1 ? false : true;
        }
    }
}