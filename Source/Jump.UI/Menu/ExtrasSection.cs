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
        }
        public override void Focus()
        {
            base.Focus();
            GetNode<ButtonMinimal>("%Credits").GrabFocus();
        }

        public void DisplayCredits()
        {
            Hide();
            menu.DisplayCreditsSection();
        }

        public void DisplayStats()
        {
            Hide();
            menu.DisplayStatsSection();
        }

        protected override void OnDisplay()
        {
            base.OnDisplay();
            PlayDisplayAnimation();
            Focus();
        }
        protected override void OnHide()
        {
            base.OnHide();
            PlayHideAnimation();
        }

        protected override void OnGoBack()
        {
            Hide();
            menu.DisplayMainSection();
        }
    }
}
