using Godot;
using GodotFmod;
using Jump.Extensions;

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
            spinBox = GetNode<SpinBox>("SpinBox");

            spinBox.Value = slider.Value;
            spinBox.MinValue = slider.MinValue;
            spinBox.MaxValue = slider.MaxValue;
            spinBox.Step = slider.Step;

            _fmodRuntime = this.GetSingleton<FmodRuntime>();

            slider.Connect("focus_entered", this, nameof(SliderFocusEntered));
            slider.Connect("mouse_entered", this, nameof(SliderFocusEntered));

            slider.Connect("value_changed", this, nameof(SliderChanged));
            spinBox.Connect("value_changed", this, nameof(SpinboxChanged));
            spinBox.Connect("focus_entered", this, nameof(SpinboxFocused));
        }

        protected virtual void OnValueChanged(float value) { }

        private void SliderChanged(float value)
        {
            _fmodRuntime.PlayOneShot("event:/UI/SliderTick");
            spinBox.Value = slider.Value;
            OnValueChanged(value);
        }

        private void SpinboxChanged(float value)
        {
            slider.Value = spinBox.Value;
            OnValueChanged(value);
        }

        private void SpinboxFocused()
        {
            spinBox.ReleaseFocus();
            slider.GrabFocus();
        }

        private void SliderFocusEntered()
        {
            _fmodRuntime.PlayOneShot("event:/UI/ButtonHover");
        }

        protected HSlider slider;
        protected SpinBox spinBox;

        private FmodRuntime _fmodRuntime;
    }
}
