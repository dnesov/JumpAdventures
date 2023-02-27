using Godot;
using System;

namespace Jump.UI.Settings
{
    public abstract class SliderProperty : SettingsProperty
    {
        public override bool IsDisabled() => false;
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            slider = GetNode<HSlider>("Value");
            slider.Connect("value_changed", this, nameof(ValueChanged));
        }

        protected virtual void ValueChanged(float value) { }

        protected HSlider slider;
    }
}
