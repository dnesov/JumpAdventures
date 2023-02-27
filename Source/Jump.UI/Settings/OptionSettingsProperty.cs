using System;
using Godot;

namespace Jump.UI.Settings
{
    public abstract class OptionSettingsProperty : SettingsProperty
    {
        public override void _Ready()
        {
            base._Ready();
            optionButton = GetNode<OptionButton>("Value");
            optionButton.Connect("item_selected", this, nameof(ValueChanged));
        }

        protected virtual void ValueChanged(int value) { }

        public override bool IsDisabled() => optionButton.Disabled;

        protected OptionButton optionButton;
    }
}