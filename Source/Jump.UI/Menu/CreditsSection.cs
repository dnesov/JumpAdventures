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
        }

        protected override void OnDisplay()
        {
            base.OnDisplay();
            PlayDisplayAnimation();

            soundtrackManager.PlayTrack(_creditsMusicPath);
        }
        protected override void OnHide()
        {
            base.OnHide();
            PlayHideAnimation();
            soundtrackManager.PlayTrack(_mainMenuMusicPath);
        }

        protected override void OnGoBack()
        {
            Hide();
            menu.DisplayExtrasSection();
        }

        private string _creditsMusicPath = "event:/Soundtrack/Credits";
        private readonly string _mainMenuMusicPath = "event:/Soundtrack/Menu";
    }
}
