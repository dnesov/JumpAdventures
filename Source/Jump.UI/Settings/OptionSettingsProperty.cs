using System;
using Godot;
using GodotFmod;
using Jump.Extensions;

namespace Jump.UI.Settings
{
    public abstract class OptionSettingsProperty : SettingsProperty
    {
        public override void _Ready()
        {
            base._Ready();

            optionButton = GetNode<OptionButton>("Value");
            optionButton.Connect("item_selected", this, nameof(ValueChanged));
            optionButton.Connect("item_selected", this, nameof(ItemPressed));
            optionButton.Connect("item_focused", this, nameof(ItemFocused));
            optionButton.Connect("mouse_entered", this, nameof(FocusEntered));
            optionButton.Connect("focus_entered", this, nameof(FocusEntered));
            optionButton.Connect("pressed", this, nameof(Pressed));

            _fmodRuntime = this.GetSingleton<FmodRuntime>();
        }

        protected virtual void ValueChanged(int value) { }

        public override bool IsDisabled() => optionButton.Disabled;

        protected OptionButton optionButton;


        private void FocusEntered()
        {
            _fmodRuntime.PlayOneShot("event:/UI/ButtonHover");
        }

        private void Pressed()
        {
            _fmodRuntime.PlayOneShot("event:/UI/ButtonPress");
        }

        private void ItemPressed(int item)
        {
            _fmodRuntime.PlayOneShot("event:/UI/ButtonPress");
        }

        private void ItemFocused(int item)
        {
            _fmodRuntime.PlayOneShot("event:/UI/ButtonHover");
        }

        private FmodRuntime _fmodRuntime;
    }
}