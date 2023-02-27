using Godot;
using System;

namespace Jump.UI.Menu
{
    public class CreditsSection : MenuSection
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            GetNode<ButtonMinimal>("%Back").OnPressedAction += GoBack;
        }

        protected override void OnDisplay() => PlayDisplayAnimation();
        protected override void OnHide() => PlayHideAnimation();

        protected override void GoBack()
        {
            Hide();
            menu.DisplayExtrasSection();
        }
    }
}
