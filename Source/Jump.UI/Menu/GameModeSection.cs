using Godot;
using System;

using Jump.Extensions;
using Jump.Misc;

namespace Jump.UI.Menu
{
    public class GameModeSection : MenuSection
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
        }

        public override void Focus()
        {
            base.Focus();
            GetNode<JAButton>("%AdventureButton").GrabFocus();
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

        private void AdventureModePressed()
        {
            Hide();
            var game = this.GetSingleton<Game>();
            var gamemode = new AdventureGameMode();

            game.SetGameMode(gamemode);
            menu.DisplayPlaySection();
        }

        private void ChallengeModePressed()
        {
            Hide();
            var game = this.GetSingleton<Game>();
            var gamemode = new ChallengeGameMode();

            game.SetGameMode(gamemode);
            menu.DisplayPlaySection();
        }

        protected override void OnGoBack()
        {
            Hide();
            menu.DisplayMainSection();
        }
    }
}
