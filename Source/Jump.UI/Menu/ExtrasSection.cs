using Godot;
using System;

namespace Jump.UI.Menu
{
    public class ExtrasSection : MenuSection
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            GetNode<ButtonMinimal>("%Back").OnPressedAction += GoBack;
            GetNode<ButtonMinimal>("%Credits").OnPressedAction += CreditsPressed;
            GetNode<ButtonMinimal>("%Customize").OnPressedAction += CustomizePressed;
        }
        public override void Focus()
        {
            base.Focus();
            GetNode<ButtonMinimal>("%Customize").GrabFocus();
        }

        protected override void OnDisplay()
        {
            PlayDisplayAnimation();
            Focus();
        }
        protected override void OnHide() => PlayHideAnimation();

        protected override void GoBack()
        {
            Hide();
            menu.ReturnToMainSection();
        }
        private void CreditsPressed()
        {
            Hide();
            menu.DisplayCreditsSection();
        }

        private void CustomizePressed()
        {
            Hide();
            menu.DisplayCustomizeSection();
        }
    }
}
