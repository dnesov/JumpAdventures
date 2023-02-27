using System;
using Godot;
using GodotFmod;

namespace Jump.UI
{
    public abstract class JAButton : TextureButton
    {
        public Action OnPressedAction;
        public Action OnFocusEnteredAction;

        public override void _Ready()
        {
            base._Ready();
            ConnectSignals();

            fmodRuntime = GetTree().Root.GetNode<FmodRuntime>("FmodRuntime");

            if (!Disabled) return;
            FocusMode = FocusModeEnum.None;
        }

        protected abstract void OnPressed();
        protected abstract void OnFocusEntered();
        protected abstract void OnFocusExited();

        private void ConnectSignals()
        {
            Connect("pressed", this, nameof(OnPressedSignal));
            Connect("focus_entered", this, nameof(FocusEntered));
            Connect("mouse_entered", this, nameof(MouseEntered));
            Connect("focus_exited", this, nameof(FocusExited));
        }

        private void FocusEntered()
        {
            OnFocusEntered();
            OnFocusEnteredAction?.Invoke();
        }

        private void MouseEntered()
        {
            if (Disabled) return;
            GrabFocus();
        }

        private void MouseExited()
        {

        }

        private void FocusExited()
        {
            OnFocusExited();
        }

        private void OnPressedSignal()
        {
            OnPressed();
            OnPressedAction?.Invoke();
        }

        [Export] protected float tweenTime = 0.15f;
        protected FmodRuntime fmodRuntime;

        protected string buttonHoverEvent = "event:/UI/ButtonHover";
        protected string buttonPressEvent = "event:/UI/ButtonPress";
    }
}